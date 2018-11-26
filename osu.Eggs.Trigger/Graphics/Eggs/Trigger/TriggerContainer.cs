using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Input.States.Handlers.Mouse;
using osu.Framework.Logging;
using osuTK.Input;

namespace osu.Framework.Graphics.Eggs.Trigger
{
    public class TriggerContainer : Container
    {
        protected virtual IList<Type> EnableTypes => 
            new List<Type>{ typeof(Key), typeof(ITriggerObject), typeof(SlideDirection) };

        public IList<object> TriggerSteps { get; set; } = new List<object>
            {
                Key.Up, Key.Up ,Key.Down,Key.Down,
                Key.Left,Key.Right,Key.Left,Key.Right,
                Key.B,Key.A
            };

        //Finish one step
        public Action<int, object> StepTrigger { get; set; }

        //Triggered, it time to show eggs!
        public Action Trigger { get; set; }

        //Reset trigger
        public Action TriggerReset { get; set; }

        private int triggerIndex;
        public int TriggerIndex
        {
            get => triggerIndex;
            protected set
            {
                if (value > TriggerSteps.Count)
                    triggerIndex = TriggerSteps.Count - 1;

                if (value < 0)
                    triggerIndex = 0;

                triggerIndex = value;
            }
        }

        private readonly Logger logger;

        protected string StepTriggerString => $"Egg step : {TriggerIndex} triggered";
        protected string TriggerString => $"Egg triggered";
        protected string TriggerResetString => $"Egg trigger reset.";

        public TriggerContainer()
        {
            logger = Logger.GetLogger();
        }

        protected override bool OnClick(ClickEvent e)
        {
            //Detect clicked object.
            var anyClickChildObject =
                TriggerSteps.OfType<ITriggerObject>().Where(x => x.IsHovered)
                    .Any(x => TriggerSteps.IndexOf(x) == TriggerIndex + 1);

            if (anyClickChildObject)
            {
                OnStepTrigger();
            }
            else
            {
                OnTriggerReset();
            }

            return base.OnClick(e);
        }

        protected override bool OnMouseUp(MouseUpEvent e)
        {
            //Detect mouse slide position
            var mouseDownPosition = e.MouseDownPosition;
            var mouseUpPosition = e.MousePosition;

            //TODO : Check Trigger

            return base.OnMouseUp(e);
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            //TODO : Check Trigger

            return base.OnKeyDown(e);
        }

        protected virtual void OnStepTrigger()
        {
            if(!TriggerSteps.Any())
                return;

            if (TriggerIndex == TriggerSteps.Count - 1)
            {
                OnTrigger();
                return;
            }

            if (!string.IsNullOrEmpty(StepTriggerString))
                logger.Add(StepTriggerString,LogLevel.Debug);

            StepTrigger?.Invoke(TriggerIndex,TriggerSteps[TriggerIndex]);

            TriggerIndex++;
        }

        protected virtual void OnTrigger()
        {
            if (!string.IsNullOrEmpty(TriggerString))
                logger.Add(TriggerString, LogLevel.Debug);

            Trigger?.Invoke();
        }

        protected virtual void OnTriggerReset()
        {
            if (!string.IsNullOrEmpty(TriggerResetString))
                logger.Add(TriggerResetString, LogLevel.Debug);

            TriggerReset?.Invoke();
        }
    }
}
