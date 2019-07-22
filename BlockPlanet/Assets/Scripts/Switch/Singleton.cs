using UnityEngine;

/// <summary>
/// �p�����邾���ŃV���O���g��������
/// </summary>
/// <typeparam name="T">�V���O���g��������N���X�̌^</typeparam>
public class Singleton<T> : MyMonoBehaviour where T : MyMonoBehaviour
{
    //�C���X�^���X
    static T m_Instance = default(T);
    //�C���X�^���X���t���O
    static bool m_IsInstantiate = false;

    void Start()
    {
        //�C���X�^���X���͈�x����
        if (!m_IsInstantiate) Instantiate();
    }

    void Update()
    {
        if (!m_IsInstantiate) Instantiate();
        m_Instance.MyUpdate();
    }

    /// <summary>
    /// �C���X�^���X�̃Q�b�^
    /// </summary>
    /// <returns>�C���X�^���X</returns>
    public static T GetInstance()
    {
        if (!m_IsInstantiate) Instantiate();
        return m_Instance;
    }

    static void Instantiate()
    {
        var instanceObjects = FindObjectsOfType(typeof(T));
        //��̂݌��������ꍇ�͐���
        if (instanceObjects.Length == 1)
        {
            //�C���X�^���X���t���O��true�ɂ���
            m_IsInstantiate = true;
            m_Instance = (T)instanceObjects[0];
            m_Instance.MyStart();
        }
        //�������������ꍇ�̓G���[���o��
        else if (instanceObjects.Length > 1)
        {
            Debug.LogError(instanceObjects[0].GetType().FullName +
            "��Component���Ă���I�u�W�F�N�g����������܂��B\n" +
            "��ɂ��Ă�������");
        }
        else
        {
            Debug.LogError("Singleton�ɂĖ��m�̃G���[:�C���X�^���X�����������܂���");
        }
    }
}

/// <summary>
/// �I�[�o�[���C�h����N���X
/// </summary>
public class MyMonoBehaviour : MonoBehaviour
{
    public virtual void MyStart() { }
    public virtual void MyUpdate() { }
}