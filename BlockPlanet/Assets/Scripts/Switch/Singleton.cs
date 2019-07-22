using UnityEngine;

/// <summary>
/// �p�����邾���ŃV���O���g��������
/// </summary>
/// <typeparam name="T">�V���O���g��������N���X�̌^</typeparam>
public class Singleton<T> : MyMonoBehaviour where T : MyMonoBehaviour
{
    //�C���X�^���X
    static T instance = default(T);
    //�C���X�^���X���t���O
    static bool isInstantiate = false;

    void Start()
    {
        //�C���X�^���X���͈�x����
        if (!isInstantiate) Instantiate();
    }

    void Update()
    {
        if (!isInstantiate) Instantiate();
        instance.MyUpdate();
    }

    /// <summary>
    /// �C���X�^���X�̃Q�b�^
    /// </summary>
    /// <returns>�C���X�^���X</returns>
    public static T GetInstance()
    {
        if (!isInstantiate) Instantiate();
        return instance;
    }

    static void Instantiate()
    {
        var instanceObjects = FindObjectsOfType(typeof(T));
        //��̂݌��������ꍇ�͐���
        if (instanceObjects.Length == 1)
        {
            //�C���X�^���X���t���O��true�ɂ���
            isInstantiate = true;
            instance = (T)instanceObjects[0];
            instance.MyStart();
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