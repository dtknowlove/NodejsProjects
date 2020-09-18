/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 ****************************************************************************/

/* TODO:Data Binding

using System.ComponentModel;
using JetBrains.Annotations;

namespace PTGame.UI.Editor
{
    using PTGame.Framework;
    using UnityEditor;
    
    [CustomEditor(typeof(PTUIMark))]
    public class PTUIMarkInspector : InspectorGUILayout,INotifyPropertyChanged
    {
        private PTUIMark mComponent;
        
        public PTUIMarkInspector()
        {
            mComponent = target as PTUIMark;
            
            this.HorizontalLayout()
                .Label().Text.Set("Support Data Binding").EndWidget()
                .Checkbox().Checked.Bind(() => SupportDataBinding).EndWidget()
                .EndWidget()
    
                .HorizontalLayout().Enabled.Bind(() => SupportDataBinding)
                .Label().Text.Set("Event Type").EndWidget()
                .DrawEvent().Event(() => { EventType = (UIBindingEventType)EditorGUILayout.EnumPopup(EventType); }).EndWidget()
                .EndWidget();
                
            BindViewModel(this);
        }

        private bool mSupportDataBinding;

        public bool SupportDataBinding
        {
            get { return mSupportDataBinding; }
            set
            {
                if (mSupportDataBinding != value)
                {
                    mSupportDataBinding = value;
                    OnPropertyChanged("SupportDataBinding");
                }
            }
        }

        private UIBindingEventType mEventType;
        public UIBindingEventType EventType
        {
            get { return mEventType; }
            set
            {
                if (mEventType != value)
                {
                    mEventType = value;
                    OnPropertyChanged("EventType");
                }
            }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

*/