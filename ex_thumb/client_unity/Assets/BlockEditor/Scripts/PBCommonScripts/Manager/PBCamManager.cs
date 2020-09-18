/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using System;
using UnityEngine;
using PTGame.Core;
using DG.Tweening;

namespace Putao.PaiBloks.Common
{
    /// <summary>
    /// 参数（默认是搭建界面的)
    /// </summary>
    public class PBCameraData
    {
        /// <summary>
        /// 初始化的位置?
        /// </summary>
        public Vector3 OriginPos = new Vector3(-16.1f, 5.47f, -0.23f);

        /// <summary>
        /// 初始化角度?
        /// </summary>
        public Vector2 OriginAngle = new Vector3(18, 90, 0);

        /// <summary>
        /// 计算结果后的偏移
        /// </summary>
        public Vector3 ResultCameraPosOffset = Vector3.zero;

        public PBCameraData()
        {
            
        }

        public PBCameraData(Vector3 _originPos,Vector2 _originAngle):this(_originPos,_originAngle,Vector3.zero)
        {
            
        }
        
        public PBCameraData(Vector3 _originPos,Vector2 _originAngle,Vector3 _offset)
        {
            OriginPos = _originPos;
            OriginAngle = _originAngle;
            ResultCameraPosOffset = _offset;
        }
        
        public void SetData(Vector3 _originPos,Vector2 _originAngle)
        {
            SetData(_originPos, _originAngle, Vector3.zero);
        }

        public void SetData(Vector3 _originPos,Vector2 _originAngle,Vector3 _offset)
        {
            OriginPos = _originPos;
            OriginAngle = _originAngle;
            ResultCameraPosOffset = _offset;
        }

        public void Description()
        {
            Debug.LogFormat("[InitPos]:{0} ", OriginPos);
        }
    }


#if BLOCK_EDITOR
    public class PBCamManager : MonoBehaviour
    {
        //NOTICE: 不用MonoSingleton，因为需要在切场景时destroy，但又想全局获取
        private static PBCamManager mInstance = null;
        public static PBCamManager Instance { get { return mInstance; } }
        
        void Awake()
        {
            mInstance = this;

            mData = new PBCameraData();
            mCamera = gameObject.GetCom<Camera>();
            mTrans = transform;
            Reset();
            SetCameraFov();
        }

        void OnDestroy()
        {
            mInstance = null;
        }
        
#else
    public class PBCamManager : PTSingleton<PBCamManager>
    {
        private PBCamManager()
        {
            
        }
        
        public void Init(Camera camera, PBCameraData data = null)
        {
            mData = data ?? new PBCameraData();
    
            mCamera = camera;
            mTrans = mCamera.transform;
            Reset();
    
            SetCameraFov();
        }
#endif
        private PBCameraData mData;
        private Transform mTrans;
        private Camera mCamera;
        private Vector3 initPos = Vector3.zero;

        private const float DEFAULT_OFFSET_Y = 0.9f;
        private const float DEFAULT_OFFSET_DEPTH = 3.5f;

        [Tooltip("block在屏幕中的view rect，百分比小数")] 
        [SerializeField] public Rect ViewRect = Rect.zero;

        public Camera GetCamera()
        {
            return mCamera;
        }

        public GameObject GetCameraObject()
        {
            return mCamera == null ? null : mCamera.gameObject;
        }
        
        public bool IsCameraFollow = true;
        
        private void SetCameraFov()
        {
            if (!mCamera.orthographic)
            {
                var ratio = Screen.width * 1.0f / Screen.height;
                var ratio2 = 16.0f / 9;
                var delta = ratio - ratio2;
                mOriginFiledOfView = Math.Abs(delta) > 0.15f ? 42 : 30;
                ResetCameraFov();
            }
        }
        
        private float mOriginFiledOfView = 0;
        public void SetCameraFov(float size)
        {
            float curSize = mCamera.fieldOfView + size;
            if (curSize >= mOriginFiledOfView)
                curSize = mOriginFiledOfView;
            if (curSize <= 15)
                curSize = 15;
            mCamera.fieldOfView = curSize;
        }
        
        public void ResetCameraFov()
        {
            mCamera.fieldOfView = mOriginFiledOfView;
        }
        
        public void Reset()
        {
 
            mTrans.position = mData.OriginPos;
            mTrans.eulerAngles = mData.OriginAngle;
        }
        
        public void MoveTo(Vector3 destPos, bool withoutanim = false)
        {
            if (Vector3.Distance(mTrans.position, destPos) < .1f)
            {
                return;
            }
            if (!withoutanim)
                mTrans.DOMove(destPos, 0.5f);
            else
                mTrans.position = destPos;
        }

        public void MoveToShowAll(Bounds bounds, bool withoutanim = false, float offsetY = DEFAULT_OFFSET_Y, float offsetDepth = DEFAULT_OFFSET_DEPTH)
        {
            initPos = CaculateCameraPos(bounds, offsetY, offsetDepth);
            MoveTo(initPos, withoutanim);
        }

        public void MoveToShowAllInPlay()
        {
            mTrans.DOMove(initPos, 0.5f);
        }

        private Vector3 CaculateCameraPos(Bounds bounds, float offsetY, float offsetDepth)
        {
            if (mCamera.orthographic)
                return CaculateCameraPos_Orthographic(bounds);

            var disOffset = offsetDepth;
            var yOffset = offsetY;

            //水平
            float angle = mCamera.fieldOfView / 2;
            float halfZ = (bounds.max.z - bounds.min.z) / 2;
            float radio = Mathf.Tan(Mathf.Deg2Rad * angle);
            float minZDis = halfZ / radio;

            float halfY = (bounds.max.y - bounds.min.y) / 2;
            float minYDis = halfY / radio;
            //Debug.Log(minZDis + "  " + minYDis);

            float minXDis = minYDis > minZDis ? minYDis : minZDis;
            minXDis += disOffset;

            Vector3 resPos = new Vector3(bounds.min.x - minXDis, bounds.center.y, bounds.center.z);

            //倾斜
            float resAngle = mTrans.eulerAngles.x;
            float disYOffset = Vector3.Distance(resPos, bounds.center) * Mathf.Tan(Mathf.Deg2Rad * resAngle);
            resPos += new Vector3(0, disYOffset - yOffset, 0);

            return resPos + mData.ResultCameraPosOffset;
        }

        private Vector3 CaculateCameraPos_Orthographic(Bounds bounds)
        {
            Debug.Log(">>>>> PBCameraManager view rect: " + ViewRect);
            
            float angle = Mathf.Deg2Rad * mCamera.transform.eulerAngles.x;
            float dist = 20;

            Vector3 boundDelta = bounds.max - bounds.min;
            float boundWidth = Mathf.Abs(boundDelta.z);
            float boundHeight = Mathf.Abs(boundDelta.y);
            float boundDepth = Mathf.Abs(boundDelta.x);
            float boundAngle = Mathf.Atan(boundHeight / boundDepth);
            float boundViewHeight = boundHeight / Mathf.Sin(boundAngle) * Mathf.Sin(boundAngle + angle);

            //calculate the orthographicSize
            float viewHeight = boundViewHeight / ViewRect.height;
            float viewWidth = viewHeight * mCamera.aspect;
            float viewWidthNeeded = boundWidth / ViewRect.width;
            if (viewWidth < viewWidthNeeded)
            {
                viewWidth = viewWidthNeeded;
                float newViewHeight = viewWidth / mCamera.aspect;
                //re-calculate height of viewrect
                ViewRect.height *= viewHeight / newViewHeight;
                ViewRect.y = 0.5f + (ViewRect.y - 0.5f) * viewHeight / newViewHeight;
                viewHeight = newViewHeight;
            }
            else if (viewWidth > viewWidthNeeded)
            {
                //re-calculate width of viewrect
                ViewRect.width *= viewWidthNeeded / viewWidth;
                ViewRect.x = 0.5f + (ViewRect.x - 0.5f) * viewWidthNeeded / viewWidth;
            }

            //The orthographicSize is half the size of the vertical viewing volume
            mCamera.orthographicSize = viewHeight * 0.5f;
            
            {
                Vector3 resPos = bounds.center - new Vector3(0, boundHeight * 0.5f, 0);
                
                //x
                float x = -dist;
                
                //y
                float deltaD = viewHeight * (ViewRect.yMax - 0.5f);
                float deltaX = deltaD * Mathf.Sin(angle);
                float deltaY = deltaD * Mathf.Cos(angle);
                float y = (dist - 0.5f * boundDepth + deltaX) * Mathf.Tan(angle) + deltaY;
                
                //z
                float z = -(ViewRect.center.x - 0.5f) * viewWidth;

                resPos += new Vector3(x, y, z);

                Debug.Log(">>>> camera pos: " + resPos);
                return resPos;
            }
        }
        
        private bool mEnablePlayModeMove = true;

        public void MoveToByKeyframe(Vector3 destPos, bool forceMove, bool withoutanim = false)
        {
            if (forceMove)
            {
                mEnablePlayModeMove = true;
            }

            if (!mEnablePlayModeMove)
            {
                return;
            }

            mEnablePlayModeMove = false;
            Vector3 d = new Vector3(-15.0f, 4.58f + destPos.y, destPos.z);

            if (Vector3.Distance(mTrans.position, d) < 1.0f)
            {
                mEnablePlayModeMove = true;
                return;
            }

            if (!withoutanim)
            {
                mTrans.DOMove(d, 0.5f).OnComplete(() => { mEnablePlayModeMove = true; });
            }
            else
            {
                mTrans.position = d;
                mEnablePlayModeMove = true;
            }
        }
        
        
        #if UNITY_EDITOR

        private bool mDrawViewRect = false;
        private Texture2D mViewRectTexture;

        public bool DrawViewRect
        {
            get { return mDrawViewRect; }
            set
            {
                if (mDrawViewRect == value)
                    return;
                mDrawViewRect = value;
                if (value)
                {            
                    if (mViewRectTexture == null)
                    {
                        mViewRectTexture = new Texture2D(2, 2);
                        Color[] pix = new Color[4];
                        for (int i = 0; i < 4; i++)
                        {
                            pix[i] = new Color(1, 0, 0, 0.5f);
                        }
                        mViewRectTexture.SetPixels(pix);
                        mViewRectTexture.Apply();
                    }
                }
            }
        }

        private void OnGUI()
        {
            if (mDrawViewRect)
            {
                GUI.DrawTexture(new Rect(ViewRect.x * Screen.width, ViewRect.y * Screen.height,
                                ViewRect.width * Screen.width, ViewRect.height * Screen.height), 
                                mViewRectTexture);
            }
        }
        
        #endif
    }
}