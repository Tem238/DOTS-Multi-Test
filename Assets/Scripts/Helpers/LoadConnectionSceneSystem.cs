#if UNITY_EDITOR

using Unity.Entities;
using UnityEngine.SceneManagement;

/// <summary>
/// エディターでどのシーンからプレイしてもロビーに飛ばしてくれる開発便利システム
/// </summary>
public partial class LoadConnectionSceneSystem : SystemBase
{
    protected override void OnCreate()
    {
        // Updateは必要ないので無効化
        Enabled = false;
        // 現在のシーンがロビーなら問題なし
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("LobbyScene")) return;
        // それ以外ならロビーをロードする
        SceneManager.LoadScene("LobbyScene");
    }

    protected override void OnUpdate()
    {
        
    }
}

#endif