/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 * http://unitylist.com/r/axc/input-system
 ****************************************************************************/

namespace PTGame.Core
{
    using System;
    using UnityEngine;

    [PTMonoSingletonPath("[Framework]/InputSystem")]
    public class InputSystem : PTMonoSingleton<InputSystem>
    {
        public Action OnKeyWDown = delegate { };
        public Action OnKeyW = delegate { };
        public Action OnKeyWUp = delegate { };
        
        public Action OnKeyADown = delegate { };
        public Action OnKeyA = delegate { };
        public Action OnKeyAUp = delegate { };
        
        public Action OnKeySDown = delegate { };
        public Action OnKeyS = delegate { };
        public Action OnKeySUp = delegate { };
        
        public Action OnKeyDDown = delegate { };
        public Action OnKeyD = delegate { };
        public Action OnKeyDUp = delegate { };
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                OnKeyWDown.InvokeGracefully(true);
            }

            if (Input.GetKey(KeyCode.W))
            {
                OnKeyW.InvokeGracefully(true);
            }

            if (Input.GetKeyUp(KeyCode.W))
            {
                OnKeyWUp.InvokeGracefully(false);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                OnKeyADown.InvokeGracefully(true);
            }
            
            if (Input.GetKey(KeyCode.A))
            {
                OnKeyA.InvokeGracefully(false);
            }
            
            if (Input.GetKeyUp(KeyCode.A))
            {
                OnKeyAUp.InvokeGracefully(false);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                OnKeySDown.InvokeGracefully(true);
            }

            if (Input.GetKey(KeyCode.S))
            {
                OnKeyS.InvokeGracefully(true);
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                OnKeySUp.InvokeGracefully(false);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                OnKeyDDown.InvokeGracefully(true);
            }

            if (Input.GetKey(KeyCode.D))
            {
                OnKeyD.InvokeGracefully(true);
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                OnKeyDUp.InvokeGracefully(false);
            }
        }
    }
}