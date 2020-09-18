using System.Collections;
using PTGame.ResKit;
using UnityEngine;

public class ExampleResKit : MonoBehaviour
{
    IEnumerator Start()
    {
        ResMgr.Instance.Init();

        //请确保 ResMgr.Instance.IsInited ！！！！！
        while (!ResMgr.Instance.IsInited)
            yield return null;

        ResLoader resLoader;

        //*******************************************************************************/
        //1. 使用ResLoader实例，加载
        resLoader = ResLoader.Allocate();

        //同步加载：只传asset名字，如果不同的assetbundle里有同名，请传assetbundle name
        resLoader.LoadSync("assetname");
        //同步加载：传asset名字 + 后缀
        resLoader.LoadSync("assetname.prefab");
        //同步加载：传asset名字 + assetbundle名字
        resLoader.LoadSync("assetname", "abname");
        //同步加载：传asset名字 + 后缀 + assetbundle名字
        resLoader.LoadSync("assetname.prefab", "abname");
        
        //异步加载，传入名字参数方法同同步，后面可传入回调函数，加载完调用
        resLoader.LoadAsync("assetname");
        resLoader.LoadAsync("assetname", onFinish: (success, asset) =>
        {
            if (success)
                Debug.Log(">>>>Finish load assetname");
        });

        //加载sprite，只提供同步加载
        resLoader.LoadSprite("spritename");
        resLoader.LoadSprite("spritename.png");
        resLoader.LoadSprite("spritename", "abname");
        resLoader.LoadSprite("spritename.png", "abname");
        
        //加载场景
        resLoader.LoadAsync("testscene.unity", onFinish: (success, scene) =>
        {
            Debug.Log(">>>>> Enter scene: testscene");
        });
        
        //从Resources下面加载，文件名：相对于Resources的相对路径，如对于路径："Resources/Shader/myshader.shader
        resLoader.LoadFromResourcesSync("Shader/myshader");
        resLoader.LoadFromResourcesAsync("Shader/myshader", (success, asset) =>
        {
            if (success)
                Debug.Log(">>>>Finish load assetname from Resources");
        });

        //加载文件，可以是：bin，text，texture，文件名：绝对地址路径，url
        resLoader.LoadFileSync("aa/bb/cc/dd/data.json");
        resLoader.LoadFileAsync("aa/bb/cc/dd/image.png", (success, content) =>
        {
            if (success)
            {
                if (content.texture != null)
                    Debug.Log(">>>>Finish load texture from url");
                else if (content.text != null)
                    Debug.Log(">>>>Finish load text from url");
                else 
                    Debug.Log(">>>>Finish load bytes from url");
            }
        });
        
        //*******************************************************************************/
        //2. 使用GlobalResLoader静态方法，加载，所有的调用方法同上
        GlobalResLoader.LoadSync("assetname");
        // ...........
        
        
        //卸载 *****************************************************************************/
        //1. 使用 ResLoader 实例，不建议以下调用
        resLoader.Release("assetname");
        resLoader.ReleaseAll();
        //建议在合适的时机调用：
        resLoader.Recycle2Cache();
        
        //*******************************************************************************/
        //2. 使用GlobalResLoader静态方法，可以调用以下方法：
        GlobalResLoader.Release("assetname");
        GlobalResLoader.ReleaseFile("aa/bb/cc/dd/data.json");
        GlobalResLoader.ReleaseFromResources("Shader/myshader");
        
        
        
    }
}
