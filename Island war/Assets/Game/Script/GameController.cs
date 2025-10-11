
using UnityEngine;
using Sirenix.OdinInspector;

public class GameController : Singleton<GameController>
{
    [Title("Manager References")]
    [SerializeField, Required] private BuidingManager buidingManager;
    
    public BuidingManager BuidingManager => buidingManager;

        public override void Awake()
    {
        // Gọi phương thức Awake của lớp cha để thực thi logic Singleton
        base.Awake();
    }

    private void Start()
    {
        // Khởi tạo các manager và thiết lập các phụ thuộc cần thiết.
        buidingManager.Initialize();
    }
}