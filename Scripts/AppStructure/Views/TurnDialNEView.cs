using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NxAppStructure;
using Loju.View;
using TMPro;

namespace AppStructure.Views
{
    public class TurnDialNEView : AbstractView 
    {
        #region Variables
        public UIKnob uiKnob;
        public TextMeshProUGUI yearText;
        #endregion

        #region Events
        public delegate void DoTurnDial(float dialValue);
        public event DoTurnDial OnTurnDial;
        public delegate void DoSnapDial(int dialSnapValue);
        public event DoSnapDial OnSnapDial;
        #endregion

        #region CustomMethods
        public void OnDragDial(float value)
        {
            OnTurnDial?.Invoke(dialValue: value);
        }
        public void OnSnapDialIntoNewPosition(int value)
        {
            SetYearText(value);
            OnSnapDial?.Invoke(dialSnapValue: value);
        }

        //I'm sure that this will be done by some kind of Era handler eventually
        private void SetYearText(int snapPos)
        {
            const int numYearSnaps = 5;
            if (snapPos >= numYearSnaps)
                return;

            string[] years = new string[numYearSnaps]
            {
                "1960",
                "1980",
                "1990",
                "2000",
                "2021"
            };
            yearText.text = years[snapPos];
        }
        #endregion

        #region RequiredMethods
        protected override void OnCreate()
        {
        }

        protected override void OnShowStart(object data)
        {
            // handle transitioning in to view
            OnShowComplete();
        }

        protected override void OnHideStart()
        {
            // handle transitioning out from view
            OnHideComplete();
        }
        #endregion
    }
}
