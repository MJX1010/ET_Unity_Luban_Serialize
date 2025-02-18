namespace ET
{
    [Invoke((long)SceneType.Main)]
    public class FiberInit_Main: AInvokeHandler<FiberInit, ETTask>
    {
        public override async ETTask Handle(FiberInit fiberInit)
        {
            Scene root = fiberInit.Fiber.Root;
           
            await EventSystem.Instance.PublishAsync(root, new EntryEvent1());   //共享
            await EventSystem.Instance.PublishAsync(root, new EntryEvent2());   //客户端
            await EventSystem.Instance.PublishAsync(root, new EntryEvent3());   //服务端
        }
    }
}