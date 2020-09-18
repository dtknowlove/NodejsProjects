//****************************************************************************
// * Copyright (c) 2017 liuzhenhua@putao.com
// ****************************************************************************/


using System;
using System.Collections;
using PTGame.Core;

#if !BLOCK_EDITOR

namespace Putao.PaiBloks.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using HighlightingSystem;
    using PTGame;
    
    /// <summary>
    /// 正常搭建播放管理类
    /// </summary>
    public class ModelManager : MonoBehaviour
    {
        private Camera BlockCamera;
        private Color mHighlightColor = new Color(1f,100f/255f,0f,1f);
        private GameObject mViewGameObject;
        private PPBlockConfigInfo mBlockConfigInfo = null;
        private Dictionary<int, GameObject> mAnimNodes = null;
        private Dictionary<int, GameObject> mViewNodes = null;
        private string mCurLayer = "Default";
        private Vector3 mCurRotateCenter=Vector3.zero;
        
        #region Propertys
        
        private List<PBPartInfo> mBlocksPartList = new List<PBPartInfo>();
        public List<PBPartInfo> BlockPartList
        {
            get { return mBlocksPartList; }
        }

        public PPBlockConfigInfo BlockConfigInfo
        {
            get { return mBlockConfigInfo; }
        }

        private bool mIsShowShadowModel = false;
        /// <summary>
        /// 是否显示模型阴影
        /// </summary>
        public bool IsShowShadowModel
        {
            get { return mIsShowShadowModel; }
            set { mIsShowShadowModel = value; }
        }

        private bool mIsShowStepHighlight = true;
        /// <summary>
        /// 是否显示步骤模型高亮
        /// </summary>
        public bool IsShowStepHighlight
        {
            get { return mIsShowStepHighlight; }
            set { mIsShowStepHighlight = value; }
        }

        private bool mIsAllowTouch = true;
        /// <summary>
        /// 是否允许交互
        /// </summary>
        public bool IsAllowTouch
        {
            get { return mIsAllowTouch; }
            set { mIsAllowTouch = value; }
        }

        private bool mIsResetPinching = true;
        /// <summary>
        /// 搭建时是否打断放大缩小
        /// </summary>
        public bool IsResetPinching
        {
            get { return mIsResetPinching; }
            set { mIsResetPinching = value; }
        }

        /// <summary>
        /// 包围盒中心
        /// </summary>
        public Vector3 BoundCenter
        {
            get { return mBlockConfigInfo.BoundBox.center; }
        }
        
        private PBKeyframeManager mPbKeyframeManager = null;
        /// <summary>
        /// 帧管理实例
        /// </summary>
        public PBKeyframeManager PbKeyframeMgr
        {
            get { return mPbKeyframeManager; }
        }
        
        private bool isEnableCtrl = false;
        /// <summary>
        /// 当前是否可控
        /// </summary>
        public bool IsEnableCtrl
        {
            get { return isEnableCtrl; }
        }

        private bool mIsEnableSetLayer = true;
        /// <summary>
        /// 是否需要设置layer
        /// </summary>
        public bool IsEnableSetLayer
        {
            get { return mIsEnableSetLayer; }
            set { mIsEnableSetLayer = value; }
        }

        private bool mIsBlockLoadedFinished = false;
        /// <summary>
        /// 是否blok完成生成
        /// </summary>
        public bool IsBlockLoadedFinished
        {
            get { return mIsBlockLoadedFinished; }
        }
        #endregion
    
        #region Unity Method
    
        void Update()
        {
            if (mIsBlockLoadedFinished && GetCurIndex() == 0)
            {
                isEnableCtrl = false;
                transform.RotateAround(mBlockConfigInfo.BoundBox.center, Vector3.up, 50 * Time.deltaTime);
            }
        }
    
        void OnDestroy()
        {
            OnPinchInOut = null;
            if (mPbKeyframeManager != null)
            {
                mPbKeyframeManager.RemoveKeyframeStart(OnKeyframeStart);
                mPbKeyframeManager.RemoveKeyframeComplete(OnKeyframeComplete);
                mPbKeyframeManager.RemoveKeyframeReset(OnKeyframeReset);
            }
            UnRegisterSwipeEvent();
        }
    
        #endregion
    
        #region Public Method

        /// <summary>
        /// 异步初始化block
        /// </summary>
        /// <param name="configFile"></param>
        /// <param name="keyframeSpeed"></param>
        /// <param name="layer"></param>
        /// <param name="onLoadBlockInfoFinish">返回值1： AnimNode个数；返回值2：keyframe个数</param>
        /// <param name="LoadblockSpeed">-1为无限制</param>
        public IEnumerator AsyncInitBlocks(string configFile, float keyframeSpeed, string layer = "Default", Action<int, int> onLoadBlockInfoFinish = null, int LoadblockSpeed = -1)
        {
            ClearData();
            InitBlockInfo(configFile);

            //keyframe第0帧不算，所以KeyfameInfos.Count - 1
            onLoadBlockInfoFinish.InvokeGracefully(mBlockConfigInfo.AnimNodeInfos.Count, mBlockConfigInfo.KeyfameInfos.Count - 1);
            
            yield return null;
            var finish = false;
            PPObjLoader.LoadBlocks(mBlockConfigInfo, transform, mAnimNodes, new PPObjLoader.LoadConfig {LoadSpeed = LoadblockSpeed}, () => finish = true);
            while (!finish)
            {
                yield return null;
            }

            mCurLayer = layer;
            transform.SetLayer(LayerMask.NameToLayer(mCurLayer));

            InitKeyframeMgr(keyframeSpeed);
            InitLight();

            yield return null;
            mIsBlockLoadedFinished = true;
        }

        /// <summary>
        /// 如果当前相机为空需重新赋值
        /// </summary>
        public void InitCamera()
        {
            if (!isInit)
                return;
            if (PBCamManager.Instance.GetCamera() == null ||
                PBCamManager.Instance.GetCamera() != BlockCamera)
            {
                PBCamManager.Instance.Init(BlockCamera);
                PBCamManager.Instance.MoveToShowAll(mBlockConfigInfo.BoundBox, true, mBlockConfigInfo.CameraOffsetY, mBlockConfigInfo.CameraOffsetDepth);
            }
        }

        private PBCameraData mCameraData = null;
        private float mFiledOfView = -1;
        private PosAngle mTranPosAngle;
        public void SaveCameraData()
        {
            mCameraData=new PBCameraData();
            mFiledOfView = -1;
            Transform cameraTran = PBCamManager.Instance.GetCamera().transform;
            mCameraData.SetData(cameraTran.position,cameraTran.eulerAngles);
            mFiledOfView = cameraTran.GetComponent<Camera>().fieldOfView;
            mTranPosAngle = new PosAngle(transform.position, transform.eulerAngles);
        }

        public void SetCameraSaveData()
        {
            if (!isInit)
                return;
            Transform cameraTran = PBCamManager.Instance.GetCamera().transform;
            if (mCameraData != null)
            {
                cameraTran.position = mCameraData.OriginPos;
                cameraTran.eulerAngles = mCameraData.OriginAngle;
            }
            if (mFiledOfView != -1)
            {
                cameraTran.GetComponent<Camera>().fieldOfView = mFiledOfView;
            }
            if (mTranPosAngle != null)
            {
                transform.position = mTranPosAngle.Pos;
                transform.eulerAngles = mTranPosAngle.Angle;
            }
            mCameraData = null;
            mFiledOfView = -1;
            mTranPosAngle = null;
        }

        public void SetCameraViewRect(Rect veiwRect)
        {
            var cameraTransform = transform.parent.Find("ClearCamera");
            if (cameraTransform == null)
                return;
            cameraTransform.Show();
            InitCamera();
            BlockCamera.rect = veiwRect;
        }

        public void OnShow()
        {
            if (BlockCamera == null)
                return;
            BlockCamera.gameObject.SetActive(true);

            if (mBlockConfigInfo != null)
                InitLight();
        }

        public void OnHide()
        {
            if (BlockCamera == null)
                return;
            BlockCamera.gameObject.SetActive(false);
        }
    
        /// <summary>
        /// 是否打开高亮阴影
        /// </summary>
        /// <param name="isShow"></param>
        public void OpenOrNotFullViewModel(bool isShow)
        {
            if (!mIsShowShadowModel) return;
            if (mViewGameObject == null) return;
            mViewGameObject.SetActive(isShow);
            Highlighter highlighter = mViewGameObject.AddSingleComponent<Highlighter>();
            if (isShow)
            {
                highlighter.ConstantOn(Color.black);
            }
            else
            {
                highlighter.ConstantOff();
            }
        }
    
        /// <summary>
        /// 是否有平面
        /// </summary>
        /// <returns></returns>
        public bool HasPlatform()
        {
            return mBlocksPartList.Any(t =>
                t.PrefabName.Equals("diban") || t.PrefabName.Equals("paibloks_base_400x400_light_coolgray"));
        }
    
        /// <summary>
        /// 当前root的显隐
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowOrHideParent(bool isShow)
        {
            transform.parent.gameObject.SetActive(isShow);
        }
    
        /// <summary>
        /// 隐藏当前已播放步骤的模型阴影
        /// </summary>
        /// <param name="isHideCur"></param>
        /// <param name="index"></param>
        public void HideAllCurViewModel(bool isHideCur = false, int index = -1)
        {
            if (index < 0)
            {
                index = mPbKeyframeManager.CurKeyFrameIndex;
            }
            List<int> idList = new List<int>();
            if (isHideCur)
            {
                for (int i = 0; i <= index; i++)
                {
                    List<int> item = mBlockConfigInfo.GetPartIDByStep(i);
                    if (item.Count > 0)
                        idList.AddRange(item);
                }
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    List<int> item = mBlockConfigInfo.GetPartIDByStep(i);
                    if (item.Count > 0)
                        idList.AddRange(item);
                }
            }
    
            idList.ForEach(id =>
            {
                if (mViewNodes[id].activeSelf)
                {
                    mViewNodes[id].SetActive(false);
                }
            });
            mViewNodes.Keys.Except(idList).ToList().ForEach(id =>
            {
                if (!mViewNodes[id].activeSelf)
                {
                    mViewNodes[id].SetActive(true);
                }
            });
        }
    
        private int preIndex = -1;
        /// <summary>
        /// 滑动条滑动时更新当前搭建步骤
        /// </summary>
        /// <param name="targetIndex"></param>
        public void SlideAction(int targetIndex)
        {
            if (targetIndex == preIndex)
            {
                return;
            }
            preIndex = targetIndex;
            if (targetIndex == 0)
            {
                FirstShowing();
                return;
            }
            if (targetIndex >= 1)
            {
                ResetAllBlocks(targetIndex);
            }
            mPbKeyframeManager.PlayWithOutAnim(targetIndex);
            OnSliderIndexChanged(targetIndex);
        }
    
        /// <summary>
        /// 播放上一步
        /// </summary>
        /// <returns></returns>
        public bool PlayPrevStep(bool isFastPlay = false)
        {
            if (!mPbKeyframeManager.PlayLast(isFastPlay))
            {
                return false;
            }
            if (GetCurIndex() == 0)
            {
                FirstShowing();
            }
            return true;
        }
    
        /// <summary>
        /// 播放下一步
        /// </summary>
        /// <returns></returns>
        public bool PlayNextStep(bool isFastPlay = false)
        {
            if (!mPbKeyframeManager.PlayNext(isFastPlay))
            {
                return false;
            }
            int curIndex = mPbKeyframeManager.CurKeyFrameIndex;
            if (curIndex == 1)
            {
                ResetAllBlocks();
            }
            return true;
        }
        
        /// <summary>
        /// 停止播放当前步
        /// </summary>
        /// <param name="isInit"></param>
        public void StopPlay(bool isInit=false)
        {
            if (isInit)
            {
                mPbKeyframeManager.ResetCurFrameToInitState();
                return;
            }
            mPbKeyframeManager.ResetCurFrameToEditorState();
        }

        public void Stop()
        {
            mPbKeyframeManager.Stop();
        }
    
        /// <summary>
        /// 播放当前步骤
        /// </summary>
        /// <returns></returns>
        public bool PlayCurrentStep()
        {
            return mPbKeyframeManager.RePlay();
        }

        public string GetCurProgress()
        {
            return string.Format("{0}/{1}", mPbKeyframeManager.CurKeyFrameIndex,
                mPbKeyframeManager.AllKeyCount);
        }

        public int GetCurIndex()
        {
            return mPbKeyframeManager.CurKeyFrameIndex;
        }

        public bool GetCurIsUnit()
        {
            return mPbKeyframeManager.CurKeyFrame != null && mPbKeyframeManager.CurKeyFrame.IsUnit;
        }

        public void SetStepHighLighting(PBKeyFrame curKeyFrame)
        {
            if (!mIsShowStepHighlight) return;
            foreach (PBKeyFrameItem item in curKeyFrame.KeyFrameItems)
            {
                if (item.PosNotChange || item.Target.name == "section_0") continue;

                PBBlock[] blocks = item.Target.GetComponentsInChildren<PBBlock>();
                for (int i = 0; i < blocks.Length; ++i)
                {
                    if (blocks[i].hide) continue;
                    Highlighter high = blocks[i].gameObject.AddSingleComponent<Highlighter>();
//                    if (high.seeThrough)
//                        high.SeeThroughOff();
                    high.ConstantOnImmediate(mHighlightColor);
                }
            }
        }

        public void HideStepHighLighting(PBKeyFrame curKeyFrame)
        {
            if (!mIsShowStepHighlight || curKeyFrame == null) return;
            foreach (PBKeyFrameItem item in curKeyFrame.KeyFrameItems)
            {
                if (item.PosNotChange || item.Target.name == "section_0") continue;

                PBBlock[] blocks = item.Target.GetComponentsInChildren<PBBlock>();
                for (int i = 0; i < blocks.Length; ++i)
                {
                    if (blocks[i].hide) continue;
                    Highlighter high = blocks[i].gameObject.AddSingleComponent<Highlighter>();
                    high.ConstantOffImmediate();
                }
            }
        }

        public List<PBPartInfo> GetKeyPartInfos(int curIndex)
        {
            return (mPbKeyframeManager.CurKeyFrame != null && curIndex <= mPbKeyframeManager.AllKeyCount &&
                    mPbKeyframeManager.AllKeyCount >= 0)
                ? GetPartInfo4BuildBlock(mBlockConfigInfo, curIndex)
                : null;
        }

        /// <summary>
        /// 获取当前模型所有零件及其个数
        /// </summary>
        /// <returns></returns>
        public List<PBPartInfo> BlockTypeInfos()
        {
            return mBlockConfigInfo.BlockInfos;
        }

        /// <summary>
        /// 是否有多种零件类型，大颗粒、小颗粒
        /// </summary>
        /// <returns></returns>
        public bool HasMultiPartType()
        {
            return mBlockConfigInfo.BlockInfos.HasMultiPartType();
        }

        /// <summary>
        /// 设置当前模型是否可控
        /// </summary>
        /// <param name="enableCtrl"></param>
        public void SetCtrlState(bool enableCtrl)
        {
            isEnableCtrl = enableCtrl;
        }

        public void SetCameraColor(Color color)
        {
            BlockCamera.backgroundColor = color;
        }

        /// <summary>
        /// 获取节点信息
        /// </summary>
        /// <returns></returns>
        public List<PBPointInfo> GetKeyPointsInfo()
        {
            return (from info in mBlockConfigInfo.KeyfameInfos
                where info.pointInfo != null && info.pointInfo.PointType == PBPointType.SectionPoint
                select info.pointInfo).ToList();
        }

        #endregion

        #region KeyframeMgr Event

        private void OnKeyframeStart(PBKeyFrame curKeyFrame)
        {
            SetKeyFrameLayer(curKeyFrame, true);
            isEnableCtrl = false;
            ResetView(mIsResetPinching);
            HideStepHighLighting(curKeyFrame.Previous);
            HideStepHighLighting(curKeyFrame);
            HideStepHighLighting(curKeyFrame.Next);
        }

        private void OnKeyframeComplete(PBKeyFrame curKeyFrame)
        {
            if (curKeyFrame.Index > 0)
            {
                isEnableCtrl = true;
            }
            OnStepComplete();
            SetStepHighLighting(curKeyFrame);
            mCurRotateCenter = curKeyFrame.GetObjCenter();
        }

        private void OnKeyframeReset(PBKeyFrame curKeyFrame)
        {
            SetKeyFrameLayer(curKeyFrame, false);
        }

        private void SetKeyFrameLayer(PBKeyFrame curKeyFrame,bool isShow)
        {
            if (!mIsEnableSetLayer) return;
            int layer = LayerMask.NameToLayer(isShow ? mCurLayer : "Ignore" + mCurLayer);

            if (!mFrameData.ContainsKey(curKeyFrame.Index))
            {
                Debug.Log("Init KeyFrameData Error!");
                return;
            }

            var curIndex = curKeyFrame.Index;
            var preObjs = new List<GameObject>();
            if (isShow)
            {
                foreach (var key in mFrameData.Keys)
                {
                    if (key <= curIndex)
                        preObjs.AddRange(mFrameData[key]);
                }
            }
            else
            {
                foreach (var key in mFrameData.Keys)
                {
                    if (key == curIndex)
                        preObjs.AddRange(mFrameData[key]);
                }
            }
            preObjs.ForEach(t => t.transform.SetLayer(layer));
        }
        
        #endregion

        #region EasyTouch Event
        
        private void RegisterSwipeEvent()
        {
            EasyTouch.On_SwipeStart += On_SwipeStart;
            EasyTouch.On_Swipe += On_Swipe;
            EasyTouch.On_SwipeEnd += On_SwipeEnd;
            EasyTouch.On_PinchIn += On_PinchIn;
            EasyTouch.On_PinchOut += On_PinchOut;
            RegisterEvent();
        }
    
        private void UnRegisterSwipeEvent()
        {
            EasyTouch.On_Swipe -= On_Swipe;
            EasyTouch.On_SwipeEnd -= On_SwipeEnd;
            EasyTouch.On_SwipeStart -= On_SwipeStart;
            EasyTouch.On_PinchIn -= On_PinchIn;
            EasyTouch.On_PinchOut -= On_PinchOut;
            UnRegisterEvent();
        }
    
        private void On_SwipeStart(Gesture gesture)
        {
    
        }
        
        private EasyTouch.SwipeDirection mPreDirction = EasyTouch.SwipeDirection.Up;
        private readonly bool mStartAllowUpDown = false;
        private void On_Swipe(Gesture gesture)
        {
            if (!mIsAllowTouch) return;
            if (!isEnableCtrl) return;
            if (gesture.touchCount == 1)
            {
                if (mPbKeyframeManager.CurKeyFrame == null) return;
                transform.RotateAround(mCurRotateCenter, Vector3.up, gesture.deltaPosition.x * -0.1f);
                if (!mStartAllowUpDown)
                    return;
                switch (gesture.swipe)
                {
                    case EasyTouch.SwipeDirection.None:
                    case EasyTouch.SwipeDirection.Other:
                    case EasyTouch.SwipeDirection.DownLeft:
                    case EasyTouch.SwipeDirection.DownRight:
                    case EasyTouch.SwipeDirection.UpLeft:
                    case EasyTouch.SwipeDirection.UpRight:
                        return;
                    case EasyTouch.SwipeDirection.Up:
                        if (mPreDirction == EasyTouch.SwipeDirection.Down || !IsClamping(transform))
                        {
                            transform.RotateAround(mCurRotateCenter, Vector3.forward, gesture.deltaPosition.y * -0.1f);
                        }
                        LimitTranAngle(transform);
                        mPreDirction = EasyTouch.SwipeDirection.Up;
                        break;
                    case EasyTouch.SwipeDirection.Down:
                        if (mPreDirction == EasyTouch.SwipeDirection.Up || !IsClamping(transform))
                        {
                            transform.RotateAround(mCurRotateCenter, Vector3.forward, gesture.deltaPosition.y * -0.1f);
                        }
                        LimitTranAngle(transform);
                        mPreDirction = EasyTouch.SwipeDirection.Down;
                        break;
                    case EasyTouch.SwipeDirection.Left:
                    case EasyTouch.SwipeDirection.Right:
                        transform.RotateAround(mCurRotateCenter, Vector3.up, gesture.deltaPosition.x * -0.1f);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        private const float MaxAngle = 300;
        private const float MinAngle = 60;
        private float ClampAngle(float angle)
        {
            return angle > 180 ? Mathf.Clamp(angle, MaxAngle, 360) : Mathf.Clamp(angle, 0, MinAngle);
        }

        private bool IsClamping(Transform trans)
        {
            return IsClampingByAngle(trans.eulerAngles.x) || IsClampingByAngle(trans.eulerAngles.z);
        }

        private bool IsClampingByAngle(float angle)
        {
            return angle <= MaxAngle && angle >= MinAngle;
        }

        private void LimitTranAngle(Transform trans)
        {
            trans.EulerAngleX(ClampAngle(trans.eulerAngles.x))
                .EulerAngleZ(ClampAngle(trans.eulerAngles.z));
        }
    
        private float angleThreshold = 3f;
        private float angleYSum = 0f;
        private float angleSpeed = -0.3f;
    
        protected virtual void RotateModel(Gesture gesture)
        {
            float angle = gesture.GetSwipeOrDragAngle();
            if ((angle > 45 && angle < 135) || (angle > -135 && angle < -45))
            {
                transform.Rotate(Vector3.forward, gesture.swipeVector.y * angleSpeed, Space.World);
            }
            else
            {
                transform.Rotate(Vector3.up, gesture.swipeVector.x * angleSpeed, Space.World);
            }
        }
    
        private void On_SwipeEnd(Gesture gesture)
        {
    
        }

        public Action OnPinchInOut = null;
        private void On_PinchIn(Gesture gesture)
        {
            if (!mIsAllowTouch) return;
            if (!isEnableCtrl) return;

            float zoom = Time.deltaTime * gesture.deltaPinch;
            
            PBCamManager.Instance.SetCameraFov(zoom);
            OnPinchInOut.InvokeGracefully();
        }

        private void On_PinchOut(Gesture gesture)
        {
            if (!mIsAllowTouch) return;
            if (!isEnableCtrl) return;
            float zoom = Time.deltaTime * gesture.deltaPinch;

            PBCamManager.Instance.SetCameraFov(-zoom);
            OnPinchInOut.InvokeGracefully();
        }

        #endregion
    
        #region Private Method

        private bool isInit = false;
        private void Init()
        {
            if (isInit)
                return;
            RegisterSwipeEvent();
            mPbKeyframeManager = gameObject.AddComponent<PBKeyframeManager>();
            BlockCamera = transform.parent.Find("Camera").GetComponent<Camera>();
            if (IsShowStepHighlight)
            {
                HighlightingRenderer highlightingRenderer = BlockCamera.gameObject.AddComponent<HighlightingRenderer>();
                highlightingRenderer.downsampleFactor = 2;
                highlightingRenderer.iterations = 5;
                highlightingRenderer.blurMinSpread = 1;
                highlightingRenderer.blurSpread = 0;
                highlightingRenderer.blurIntensity = 0.34f;
            }
            isInit = true;
        }

        private void FirstShowing()
        {
            mPbKeyframeManager.PlayWithOutAnim(0);
            transform.SetLayer(LayerMask.NameToLayer(mCurLayer));
            ResetView(true);
            mPbKeyframeManager.ResetIndex();
            ShowEditorPosition();
            PBCamManager.Instance.MoveToShowAll(mBlockConfigInfo.BoundBox, true, mBlockConfigInfo.CameraOffsetY, mBlockConfigInfo.CameraOffsetDepth);
            foreach (PBKeyFrame keyFrame in mPbKeyframeManager.KeyFrames)
            {
                HideStepHighLighting(keyFrame);
            }
            if (mIsShowShadowModel)
                OpenOrNotFullViewModel(mViewGameObject.activeSelf);
        }
    
        private void ShowEditorPosition()
        {
            mAnimNodes.Values.ToList().ForEach(t =>
            {
                t.GetComponent<PBAnimNode>().SetEditorTransform();
            });
        }
    
        private void ResetAllBlocks(int resetIndex=1)
        {
            mPbKeyframeManager.Reset(resetIndex + 1);
        }
    
        protected void HideCurViewModel()
        {
            List<int> idList = mBlockConfigInfo.GetPartIDByStep(mPbKeyframeManager.CurKeyFrameIndex);
    
            idList.ForEach(id =>
            {
                if (mViewNodes[id].activeSelf)
                    mViewNodes[id].SetActive(false);
            });
        }
        
        private void ResetView(bool isResetPinching)
        {
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            if (!transform.localEulerAngles.Equals(Vector3.zero))
            {
                transform.localEulerAngles = Vector3.zero;
            }
            if (isResetPinching)
                PBCamManager.Instance.ResetCameraFov();
        }
    
        /// <summary>
        /// 加载并设置阴影材质
        /// </summary>
        /// <param name="renderers"></param>
        private void ChangeMaterialToTransparent(MeshRenderer[] renderers)
        {
            Material transparenceMat = PBResLoader.Instance.Load<Material>("tp_white3");
            Material[] matAry;
            foreach (MeshRenderer renderer in renderers)
            {
                matAry = renderer.materials;
                for (int i = 0; i < matAry.Length; i++)
                {
                    matAry[i] = transparenceMat;
                }
                renderer.materials = matAry;
            }
        }
        
        /// <summary>
        /// 搭建获取零件特殊处理 slider会跳值 零件显示当前步骤没有零件不刷新
        /// </summary>
        /// <param name="blockConfigInfo"></param>
        /// <param name="frameIndex"></param>
        /// <returns></returns>
        private List<PBPartInfo> GetPartInfo4BuildBlock(PPBlockConfigInfo blockConfigInfo, int frameIndex)
        {
            List<PBPartInfo> info = null;
            for(int i=frameIndex;i >= 0;--i)
            {
                info = blockConfigInfo.GetPartInfo (i);
                if(info.Count > 0)
                {
                    return info;
                }
            }
            return info;
        }
        
        private void InitBlockInfo(string configFile)
        {
            Init();
            if (transform.childCount > 0)
            {
                transform.DestroyAllChild();
            }
            mBlockConfigInfo = PBBlockConfigManager.LoadBlockInfos(configFile);
            mBlocksPartList = mBlockConfigInfo.GetAllPartInfo();

            mAnimNodes = new Dictionary<int, GameObject>();
        }

        private void InitKeyframeMgr(float speed)
        {
            mPbKeyframeManager.Init(mBlockConfigInfo.KeyfameInfos, mAnimNodes);
            SetFrameData();
            mPbKeyframeManager.SetSpeed(speed);
            mPbKeyframeManager.RegisterKeyframeStart(OnKeyframeStart);
            mPbKeyframeManager.RegisterKeyframeComplete(OnKeyframeComplete);
            mPbKeyframeManager.RegisterKeyframeReset(OnKeyframeReset);
            foreach (var keyFrame in mPbKeyframeManager.KeyFrames)
            {
                SetStepHighLighting(keyFrame);
                HideStepHighLighting(keyFrame);
            }
            //设置相机
            PBCamManager.Instance.Init(BlockCamera);
            PBCamManager.Instance.MoveToShowAll(mBlockConfigInfo.BoundBox, true, mBlockConfigInfo.CameraOffsetY, mBlockConfigInfo.CameraOffsetDepth);
            ShowEditorPosition();
        }

        private void InitLight()
        {
            GameObject lightObj = PBLighting.LoadByName_App(mBlockConfigInfo.LightName);
            lightObj.transform.parent = this.transform.parent;
            lightObj.transform.position = Vector3.zero;
            lightObj.transform.rotation = Quaternion.identity;
            lightObj.transform.localScale = Vector3.one;
            lightObj.transform.SetLayer(LayerMask.NameToLayer(mCurLayer));
        }

        private Dictionary<int, List<GameObject>> mFrameData = new Dictionary<int, List<GameObject>>();
        /// <summary>
        /// 处理层的设置问题
        /// </summary>
        private void SetFrameData()
        {
            if(mPbKeyframeManager==null)
                return;
            if (mFrameData == null)
                mFrameData = new Dictionary<int, List<GameObject>>();
            foreach (var frame in mPbKeyframeManager.KeyFrames)
            {
                SetSingleFrameData(frame);
            }
        }

        private void SetSingleFrameData(PBKeyFrame frame)
        {
            if (!mFrameData.ContainsKey(frame.Index))
            {
                mFrameData.Add(frame.Index, new List<GameObject>());
            }
            foreach (var item in frame.KeyFrameItems)
            {
                if (item.Target.GetComponent<PBBlock>() != null)
                {
                    mFrameData[frame.Index].Add(item.Target);
                }
            }
        }

        private void ClearData()
        {
            if (mFrameData != null)
            {
                mFrameData.Clear();
                mFrameData = null;
            }
            if (mAnimNodes != null)
            {
                mAnimNodes.Clear();
                mAnimNodes = null;
            }
            if (mViewNodes != null)
            {
                mViewNodes.Clear();
                mViewNodes = null;
            }
            if (mBlocksPartList != null)
            {
                mBlocksPartList.Clear();
                mBlocksPartList = null;
            }
        }
    
        #endregion
        
        #region Protected Method
    
    
        protected virtual void OnStepComplete()
        {
    
        }
    
        protected virtual void OnSliderIndexChanged(int index)
        {
    
        }
    
        protected virtual void RegisterEvent()
        {
    
        }
    
        protected virtual void UnRegisterEvent()
        {
    
        }
    
        #endregion
    }
}

#endif