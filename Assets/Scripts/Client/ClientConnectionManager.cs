using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// �ڑ����[�h
/// </summary>
public enum ConnectionMode : byte
{
    /// <summary>�T�[�o�[�{�N���C�A���g</summary>
    ServerClient = 0,
    /// <summary>�T�[�o�[�̂�</summary>
    Server,
    /// <summary>�N���C�A���g�̂�</summary>
    Client,
}

public class ClientConnectionManager : MonoBehaviour
{

    #region �萔

    private const string MAIN_SCENE_NAME = "TestScene";
    private const string SERVER_WORLD_NAME = "Test Server";
    private const string CLIENT_WORLD_NAME = "Test Client";
    private const string CONNECTION_MODE_ERROR_MESSAGE = "���݂��Ȃ��ڑ����[�h���w�肳��Ă��܂��B";

    #endregion

    #region �C���X�y�N�^�[�ɕ\�������ϐ�

    // ���r�[��UI�Q���C���X�y�N�^�[�Őݒ肵�Ă���
    [SerializeField] private TMP_InputField _addressField;
    [SerializeField] private TMP_InputField _portField;
    [SerializeField] private TMP_Dropdown _connectionModeDropdown;
    [SerializeField] private Button _connectButton;

    #endregion

    /// <summary>
    /// �|�[�g�ԍ�(ushort)
    /// </summary>
    /// <remarks>���͂��ꂽ�|�[�g�ԍ���ushort�ɕϊ����ĕԋp</remarks>
    private ushort Port => ushort.Parse(_portField.text);

    #region �����Ăяo������郁�\�b�h

    private void OnEnable()
    {
        // Start�{�^����OnButtonConnect���\�b�h��o�^
        _connectButton.onClick.AddListener(OnButtonConnect);
    }

    private void OnDisable()
    {
        // Start�{�^���̃C�x���g��S�č폜
        _connectButton.onClick.RemoveAllListeners();
    }

    #endregion

    #region �v���C�x�[�g���\�b�h

    private void OnButtonConnect()
    {
        // �f�t�H���g�̃��[���h��S�Ĕj��
        DestroyLocalSimulationWorld();
        // �J�ڐ�̃V�[�������w�肵�ă��[�h
        SceneManager.LoadScene(MAIN_SCENE_NAME);

        // �ڑ����[�h�ɉ����ă��[���h���쐬
        switch (_connectionModeDropdown.value)
        {
            case (byte)ConnectionMode.ServerClient:
                StartServer();
                StartClient();
                break;
            case (byte)ConnectionMode.Server:
                StartServer();
                break;
            case (byte)ConnectionMode.Client:
                StartClient();
                break;
            default:
                Debug.LogError(CONNECTION_MODE_ERROR_MESSAGE, gameObject);
                break;
        }
    }

    private void DestroyLocalSimulationWorld()
    {
        foreach (var world in World.All)
        {
            if (world.Flags == WorldFlags.Game)
            {
                world.Dispose();
                break;
            }
        }
    }

    private void StartServer()
    {
        // �T�[�o�[���[���h�쐬
        var serverWorld = ClientServerBootstrap.CreateServerWorld(SERVER_WORLD_NAME);

        // �w�肳�ꂽ�|�[�g�Őڑ��ł���悤�ɂ���
        var serverEndpoint = NetworkEndpoint.AnyIpv4.WithPort(Port);
        {
            using var networkDriverQuery = serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(serverEndpoint);
        }
    }

    private void StartClient()
    {
        // �N���C�A���g���[���h�쐬
        var clientWorld = ClientServerBootstrap.CreateClientWorld(CLIENT_WORLD_NAME);

        // �T�[�o�[���[���h�̃G���h�|�C���g�ɐڑ�
        var connectionEndpoint = NetworkEndpoint.Parse(_addressField.text, Port);
        {
            using var networkDriverQuery = clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager, connectionEndpoint);
        }

        // �T�[�o�[�ڑ����N�G�X�g�p�G���e�B�e�B���쐬
        var connectRequestEntity = clientWorld.EntityManager.CreateEntity();
        // �킩��₷���悤�ɏ������ꂽ�ꏊ(10, 0, 0)���X�|�[���n�_�Ɏw��
        clientWorld.EntityManager.AddComponentData(connectRequestEntity, new ClientConnectRequest
        {
            SpawnPosition = new float3(10, 0, 0)
        });
    }

    #endregion
}
