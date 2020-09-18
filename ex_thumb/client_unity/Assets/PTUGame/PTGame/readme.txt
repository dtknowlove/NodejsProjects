
-------------2017-10-12-----------------
PTUGame_V3.3.5.unitypackage wanzhenyu@putao.com
1.更新ptgame_editor.dll 自动下载导入插件


-------------2017-09-09-----------------
PTUGame_V3.3.4.unitypackage wanzhenyu@putao.com
1.Android添加 ShowSystemAlertWithConfirm（）
2.Android添加 ShowToast（）


-------------2017-09-05-----------------
PTUGame_V3.3.3.unitypackage wanzhenyu@putao.com
1.添加启动第三方应用代码
2.添加逻辑intent getdata()

-------------2017-08-29-----------------
PTUGame_V3.3.2.unitypackage wanzhenyu@putao.com
1.添加读取相册图片相关java代码


-------------2017-06-06-----------------
PTUGame_V3.3.1.unitypackage wanzhenyu@putao.com
1.添加Android点击两次退出应用 PTAndroidExitApp

-------------2017-06-05-----------------
PTUGame_V3.3.0.unitypackage wanzhenyu@putao.com
1.修复PTUtil bug


-------------2017-06-01-----------------
PTUGame_V3.2.9.unitypackage wanzhenyu@putao.com
1.Android 添加教育版判断


-------------2017-04-17-----------------
PTUGame_V3.2.8.unitypackage wanzhenyu@putao.com
1.Android 添加点击两次退出应用统一弹框
2.Android 添加java退出应用接口
3.Android 修改启动图片为白色，为了播放视频splash。


-------------2017-04-17-----------------
PTUGame_V3.2.7.unitypackage wanzhenyu@putao.com
1.Android 启动Splash闪屏问题


-------------2017-03-08-----------------
PTUGame_V3.2.6.unitypackage wanzhenyu@putao.com
1.修复ptugame切换场景销毁问题

-------------2017-03-08-----------------
PTUGame_V3.2.5.unitypackage wanzhenyu@putao.com
1.修复插件检测，不红色显示需要更新组件的问题。
2.移除ptweidu ptgpush ptassist等手动挂载，改为自动挂载。
3.讲dll改为源码


-------------2017-1-24-----------------
PTUGame_V3.2.4.unitypackage
1.修复android平台读取配置文件问题
2.重命名ptlog.dll 为 ptdebug.dll

-------------2017-1-20-----------------
PTUGame_V3.2.3.unitypackage
1.加入projecttag 平台类游戏解决方案
2.游戏启动时检测到大版本更新。删除putaogame目录下所有文件。
3.加入访问配置文件的接口PTUGame.instance.GetConfigContent（ㄉ4.加入ptdebug
5.修改android版孩子帐号库，解决提交googl play openssl问题

-------------2017-1-11----------------- wanzhenyu@putao.com duanyiliang@putao.com
PTUGame_V3.2.2.unitypackage
1.添加Android上解决unity的一个bug:弹出键盘后webcamtexture会卡顿。
  调用：ptandroidinterface.instance.ExecuteKeyoardAdjust();

-------------2016-12-29----------------- wanzhenyu@putao.com
PTUGame_V3.2.1.unitypackage
1.张遇春王晓鹏组修复孩子帐号连接paigoWi-Fi崩溃的问题。

-------------2016-12-13----------------- wanzhenyu@putao.com
PTUGame_V3.2.0.unitypackage
1.添加android版保存到相册 PTUniInterface
2.修改方法 GetBuildNum() 为 GetBuildNumOrVersionCode()。兼容Android版获取VersionCode


-------------2016-11-29----------------- wanzhenyu@putao.com
PTUGame_V3.1.9.unitypackage
1.添加android版本更新提示框
2.修改ptugame读取配置部分，改为支持热更
3.ptugame读取配置部分，改为同步读取
4.静态 putaoUniqueDeviceId 修改 为PTUGame.Instance.putaoUniqueDeviceId,  


-------------2016-11-09----------------- wanzhenyu@putao.com
PTUGame_V3.1.8.unitypackage
1.修复paibot旋转角度问题

-------------2016-09-29----------------- wanzhenyu@putao.com
PTUGame_V3.1.7.unitypackage
1.jar中添加了游戏启动时停止葡萄听听播放

-------------2016-09-26----------------- wanzhenyu@putao.com
PTUGame_V3.1.6.unitypackage
1.添加GetPTDeviceType()方法，用来获取是paibot，paipad,
2.饩鲇镆艉舫霰喑蘡rash问题。在resume里面加了delay 1秒再调用旋转。manifest中添加了launchmode=singleinstance

-------------2016-09-21----------------- wanzhenyu@putao.com
PTUGame_V3.1.5.unitypackage
1.替换孩子帐号插件，Android孩子扫码SDK更新，修改错别字和手机平板提示文字不同的问题。
2.修改孩子帐号横屏模式没有关闭按钮。


-------------2016-09-13----------------- wanzhenyu@putao.com
PTUGame_V3.1.4.unitypackage
1.修改插件服务器地址

-------------2016-09-02----------------- wanzhenyu@putao.com
PTUGame_V3.1.3.unitypackage
1.android版孩子账号

-------------2016-08-30----------------- wanzhenyu@putao.com
PTUGame_V3.1.2.unitypackage
1.薷磒aibot上输入框问题

-------------2016-08-30----------------- wanzhenyu@putao.com
PTUGame_V3.1.1.unitypackage
1.修改有新版本更新提示相关接口。放到systemcallback里面。
2.添加系统提示接口。showsystemalertwithconfirm（）



-------------2016-08-25----------------- wanzhenyu@putao.com
PTUGame_V3.1.0.unitypackage
1.android添加判断是否在底座上

-------------2016-08-23----------------- wanzhenyu@putao.com
PTUGame_V3.0.9.unitypackage
1.android版获取版本号

-------------2016-08-16----------------- wanzhenyu@putao.com
PTUGame_V3.0.8.unitypackage
1.在checkupdate 编辑器面板中加入scrollview

-------------2016-08-10----------------- wanzhenyu@putao.com
PTUGame_V3.0.7.unitypackage
1.加入android manifest.xml增加相机权限
2.android 版本 ptgame.jar中增优断有没有paibot旋转功能，确保在非paibot上也可以运行


-------------2016-08-06----------------- wanzhenyu@putao.com
PTUGame_V3.0.6.unitypackage
1.加入android manifest.xml中竖屏


-------------2016-08-01----------------- wanzhenyu@putao.com
PTUGame_V3.0.5.unitypackage
1.加入paibot自动转向


-------------2016-08-01----------------- wanzhenyu@putao.com
PTUGame_V3.0.4.unitypackage
1.兼容Android编译


-------------2016-06-23----------------- wanzhenyu@putao.com
PTUGame_V3.0.3.unitypackage
1.修改编辑器里面默认的device id.苊獾?鲂菹⒍??

-------------2016-06-23----------------- wanzhenyu@putao.com
PTUGame_V3.0.2.unitypackage
1.修复资源更新无回调的bug

-------------2016-06-21----------------- wanzhenyu@putao.com
PTUGame_V3.0.1.unitypackage
1.麦克风，相册，摄像啡ㄏ蓦.mm文件中 ios版本更改。


-------------2016-06-06-----------------
PTUGame_V3.0.0.unitypackage
1.分离出ptgame ,ptweidu,ptwww.
2.代码部分生成dll.
3.重构配置文件自动加载部分。
4.weidu ，gpush等通过挂在相应的脚本来开簟⑸5.添加远程插件版本检测

-------------2016-05-23-----------------
PTUGame_V3.0.0.unitypackage
1.分离纬度相关哪谌荨＝?嗷?ㄏ藜觳猓?嗖崛ㄏ藜觳猓?罂朔缛ㄏ藜觳獾裙?媚谌莘诺叫陆ǖ膒tgame目录中


-------------2016-05-23-----------------
PTUGame_V2.2.0.unitypackage
1.引入PTWWW。加入www请求超时。
2.修改版本更新提示基于PTWWW.设置3秒超时限制。
3.修改家长控制。修复当天次数用完，重启后不锁屏的问题。


-------------2016-05-17-----------------
PTUGame_V2.1.9.unitypackage
1.修改新版本?绿崾荆??猘pp store.中英文版。
2.添加摄像头权限提示，跳转设置。 中英文版。

-------------2016-04-28-----------------
PTUGame_V2.1.8.unitypackage
1.修改纬度本地缓存，煌琣pp 版本存贮到不同文件中
2.优化家长控制没有网络情况下重连

-------------2016-04-27-----------------
PTUGame_V2.1.7.unitypackage
1.修改userId重新获取逻辑
 useID 获取方式修改下：
    9.每收到一次推送请求。则先判蟯ser id是否为-1. 如果为-1.则先重新获取user id. 连续尝试5次。成功则推数据，失败则放弃推。
    10.收到推送请求时，如果正在请求user id ,且不满5次。则此次推送不触发请求user id.
    11.游戏启动时，也是触发5次user id肭竽逻箕Ｈ绻?晒ΑＴ蛲扑捅镜鼗捍媪斜碇械摹⑸
-------------2016-04-26-----------------
PTUGame_V2.1.6.unitypackage
1.纬度缓存需求更改。如?捍婕录猖?0条。则新的推送不再推送放入队列后面，同时移除队列第一个元素。



-------------2016-04-25-----------------
PTUGame_V2.1.5.unitypackage
1.BundleVersionBindings.cs中添加获取build num接口
2.修复家长控制休息时常（调试忘记没厝ィㄉ
-------------2016-04-24-----------------
PTUGame_V2.1.3.unitypackage
如果没有连接?缁蛘叻⑺褪О茉蚧捍嬖诒镜亍Ｑ?分胤⒄庖惶酰?钡匠晒Α⑸根据游戏逻辑提送的次序上传。上一条如果失埽?蚝笮?幕嵋来闻哦拥却??钡缴弦惶醭晒Α⑸游戏仄糁?。会继晓⑺蜕洗位捍娴氖?荨Ｐ麓シ⒌氖?荩?嵋来闻哦拥胶竺妗⑸当游戏版本升级的时候，会清楚缓存记录。
缓存是以文本文件记录在本地。
捍嬷械牡谝惶跫锹迹?绻?胤⒋问???，则不在重发，钡浇邮艿揭淮涡碌耐扑颓肭蟆Ｔ蚩?贾胤ⅰ⑸绻?捍婕吐猖?0条。则新耐扑筒辉偻扑停?辉傩慈牖捍妗⑸
失败情况包括：启动时获取user id 不正确；没有网络连樱煌?绱砦蟆⑸
启动时获取user id 失败。则后续会持?袢。股每隔30秒重新获取一次user id.
如果重新获取的次数超?舷?，则不再获取⑸得鞲每突Ф嗣挥辛?油?缁蛘其漱?颍?挥斜匾?偌绦?取?姆研阅堋⑸


--------------2016-04-22-----------------
PTUGame_V2.1.2.unitypackage
1.添加纬度数据上传缓存机制。
a.如果没有连接网络或者发送失败则缓嬖诒镜亍Ｑ?分胤⒄庖惶酰?钡匠晒Α⑸b.根据游戏逻?说拇涡蛏铣?上一条如羹О埽?蚝笮?幕嵋来闻哦拥却??钡缴弦条成功、蒫.游戏重启之后。会继续发送上次缓存的数据。新触发的?荩?嵋来闻哦拥胶竺妗⑸d.当游戏版本升级的时候，会宄?捍媲录、蒭.缓存是以文本文件记录在本地。
2.修改重复获取user id部分。
3.修改socket连接部分。
4.json 解析部分加入try catch. 


--------------2016-04-21-----------------
PTUGame_V2.1.1.unitypackage
1.薷奈扯壬洗??萁涌冢?С掷┱埂⑸
--------------2016-04-20-----------------
PTUGame_V2.1.0.unitypackage
1.如果启动游戏时user id获取失败，则每隔五秒钟再获取一次，钡絬serid不为－1；
2.如果userid 为 ?不洗??荨⑸3.新版二维码扫描UI部分
4.用spine做小Q休息动画.

--------------2016-04-19-----------------
PTUGame_V2.0.11.unitypackage
1.游戏启动获取配置play_times 修改为玩的次数限制。game_times为游戏时间限制。
2.谑战的socket通知中修改条件判断 if (isSameProduct) 为  if (isSameProduct || useNum == -1 || useTime == -1)



--------------2016-04-18-----------------
PTUGame_V2.0.10.unitypackage
1.新版纬度功能

--------------2016-04-08-----------------
PTUGame_V2.0.9.unitypackage
1.修复断线重连等待时间过长问题
2.修复休眠状态接受指令直接解锁问题
3.添加获取GetBundleBuildIOS 在BundleVersionPlugin.mm中


--------------2016-03-25-----------------
PTUGame_V2.0.8.unitypackage
1.修复纬度长连接断网重连功能。
2.修改相机权限在ios7上弹窗提示跳转问狻⑸3. 家长控制中邮芗页ば畔⒑筇砑优卸芜if (this.state!=STATE_LOCK_NUM|| useNum<InitUseNum) {   GoToPlay();}


--------------2016-03-22-----------------
PTUGame_V2.0.7.unitypackage
1.添加 ConnectHandler（NetworkPacket msg） 中检测msg是否为空，在 PTConnecter.cs中。
2.PTParentControl 中修改 public 为  private byte state = STATE_PLAY。
3.小Q图片序列动画打图集。

---------------2016-03-07---------------
PTUGame_V2.0.6.unitypackage
1.修改家长控制的锁屏界面层级在二维码扫描界面之上。
2.重构PTUGame结构?奖阏?掀渌?寮??础?分离出配置文件，岸??虢缑娴淄肌０姹靖?虏毁需瑶婊辉应文箕⑸
----------------2016-2-26---------------
PTUGame_V2.0.5.unitypackage
1.二维码页面 “添加正在生成中，请耐心等待”
2.修改二维码请求为每点击一次请求淮巍⑸3.移除parent control中 eventsystem 屏蔽功能。

----------------2016-2-25---------------
PTUGame_V2.0.4.unitypackage
1.重构二维码面板，改为UGUI实现
2.提供涌龠 PTWeiDu.cs 中  public void ShowQRCheckerPanel(Action closeCallBack) {} 出发关闭事件共外部调用。
3.移除??朊姘逯衑ventsystem屏蔽事件。（UGUI远?沧×撕竺鎁I的事件）

----------------2016-2-22---------------
PTUGame_V2.0.3.unitypackage
1.柚週ockScreen Canvas Sort Layer 为1000
2.fix bug when connect failed partent control .return (-1,-1,-1). change to return (0,0,5).默认不无限。
3.修改纬度玩的次数耐馔?刂放渲律  <playinfo enable="true" default="online" >
    <inner url="http://api.weidu.start.wang/get/configure"/>
    <online url="http://api-weidu.putao.com/get/configure"/>
  </playinfo>
4.修复bug:删掉进程重新开始lock时间清除掉了。



------------------2016-2-19---------------
PTUGame_V2.0.2.unitypackage
1.添加发送ping之疤砑觭ession.isConnect判断。
2.ping的发送涓舾奈?。
3.修改家长控撇糠致呒?⑸4.PTUGame.cs中添加变量isPTUGameReady。
5.修改二维码趁婀乇瞻磁サ慊髑?大　⑷褥------------------2016-2-17--------------  
PTUGame_V2.0.0_beta.unitypackage
新版本的PTUGame插件已经发给各项目组负责人Ｕ飧霭姹浸氯缦拢股1.加入了家长端控制功能
2.重构了各个内外网设置。现在统一通过配梦募?４?肜锩孓需芍谩〓重构了keychain获取device id部分。现在通过配置募?柚谩⑸4.加入了一键打包，不再需要对XCODE进行各稚柚谩