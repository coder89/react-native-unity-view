using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;
using ReactNative.Bridge;
using ReactNative.UIManager;
using ReactNative.UIManager.Events;
using RNUnityViewBridge;

namespace RNUnityView
{
    public class UnityViewManager : SimpleViewManager<UnityView>, ILifecycleEventListener
    {
        private const string REACT_CLASS = "UnityView";

        public const int COMMAND_POST_MESSAGE = 1;
        public const int COMMAND_PAUSE = 2;
        public const int COMMAND_RESUME = 3;

        private static bool DONOT_RESUME = false;

        private ReactContext context;

        public UnityViewManager(ReactContext context)
        {
            this.context = context;
            this.context.AddLifecycleEventListener(this);
        }

        public override string Name => REACT_CLASS;

        public override JObject ViewCommandsMap =>
            new JObject
            {
                { "postMessage", COMMAND_POST_MESSAGE },
                { "pause", COMMAND_PAUSE },
                { "resume", COMMAND_RESUME }
            };

        public override void ReceiveCommand(UnityView view, int commandId, JArray args)
        {
            switch (commandId)
            {
                case COMMAND_POST_MESSAGE:
                    String gameObject = args[0].ToString();
                    String methodName = args[1].ToString();
                    String message = args[2].ToString();
                    UnityUtils.PostMessage(gameObject, methodName, message);
                    break;
                case COMMAND_PAUSE:
                    UnityUtils.Player.Pause();
                    DONOT_RESUME = true;
                    break;
                case COMMAND_RESUME:
                    UnityUtils.Player.Resume();
                    DONOT_RESUME = false;
                    break;
            }
        }

        protected override UnityView CreateViewInstance(ThemedReactContext reactContext)
        {
            UnityView view = new UnityView(reactContext);
            UnityUtils.AddUnityEventListener(view);
            view.AddOnAttachStateChangeListener(this);
            return view;
        }


        public override void OnDropViewInstance(ThemedReactContext reactContext, UnityView view)
        {
            UnityUtils.RemoveUnityEventListener(view);
            view.RemoveOnAttachStateChangeListener(this);
            base.OnDropViewInstance(reactContext, view);
        }

        public override JObject CustomDirectEventTypeConstants =>
            new JObject
            {
                {
                    UnityMessageEvent.EVENT_NAME,
                    new JObject
                    {
                        { "registrationName", "onMessage" }
                    }
                }
            };

        public void OnSuspend()
        {
            UnityUtils.Player.Pause();
        }

        public void OnResume()
        {
            if (!UnityUtils.IsInitialized)
            {
                UnityUtils.CreatePlayer();
            }
            else
            {
                if (!DONOT_RESUME)
                {
                    UnityUtils.Player.Resume();
                }
            }
        }

        public void OnDestroy()
        {
            UnityUtils.Player.Quit();
        }

        //    @Override
        //    public void onViewAttachedToWindow(View v)
        //    {
        //        // restore the unity player state
        //        if (DONOT_RESUME)
        //        {
        //            Handler handler = new Handler();
        //            handler.postDelayed(new Runnable() {
        //                @Override
        //                public void run()
        //            {
        //                UnityUtils.getPlayer().pause();
        //            }
        //        }, 300); //TODO: 300 is the right one?
        //    }
        //}

        //@Override
        //    public void onViewDetachedFromWindow(View v)
        //{

        //}//    @Override
        //    public void onViewAttachedToWindow(View v)
        //    {
        //        // restore the unity player state
        //        if (DONOT_RESUME)
        //        {
        //            Handler handler = new Handler();
        //            handler.postDelayed(new Runnable() {
        //                @Override
        //                public void run()
        //            {
        //                UnityUtils.getPlayer().pause();
        //            }
        //        }, 300); //TODO: 300 is the right one?
        //    }
        //}

        //@Override
        //    public void onViewDetachedFromWindow(View v)
        //{

        //}
        protected static void dispatchEvent(UnityView view, Event @event)
        {
            //ReactContext reactContext = (ReactContext)view.getContext();
            //    EventDispatcher eventDispatcher = 
            //            reactContext.getNativeModule(UIManagerModule.class).getEventDispatcher();
            //eventDispatcher.dispatchEvent(event);
        }
    }
}
