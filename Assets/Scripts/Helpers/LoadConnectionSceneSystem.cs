#if UNITY_EDITOR

using Unity.Entities;
using UnityEngine.SceneManagement;

/// <summary>
/// �G�f�B�^�[�łǂ̃V�[������v���C���Ă����r�[�ɔ�΂��Ă����J���֗��V�X�e��
/// </summary>
public partial class LoadConnectionSceneSystem : SystemBase
{
    protected override void OnCreate()
    {
        // Update�͕K�v�Ȃ��̂Ŗ�����
        Enabled = false;
        // ���݂̃V�[�������r�[�Ȃ���Ȃ�
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("LobbyScene")) return;
        // ����ȊO�Ȃ烍�r�[�����[�h����
        SceneManager.LoadScene("LobbyScene");
    }

    protected override void OnUpdate()
    {
        
    }
}

#endif