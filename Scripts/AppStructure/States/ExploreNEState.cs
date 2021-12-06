using AppStructure.Data;
using AppStructure.Views;
using Nexus.NEPeopleGroups.Runtime;
using Nexus.NEPeopleData.Runtime;
using UnityEngine;
using UnityEngine.Assertions;

namespace AppStructure.States
{
    public class ExploreNEState : MonoBehaviour
    {
        #region Variables
        private AppManager _appManager;
        private ExploreNEView _exploreNEView;
        private TurnDialNEView _turnDialNEView;
        #endregion

        #region CustomMethods

        private void ShowDial()
        {
            if (_context.ViewController.IsOverlayOpen<TurnDialNEView>())
                return;

            //Show the dial overlay...
            _context.ViewController.OpenOverlay<TurnDialNEView>(
                data: null,
                waitForAllOverlaysToClose: false);

            _turnDialNEView = (TurnDialNEView)_context.ViewController.GetOverlay<TurnDialNEView>();
            Assert.IsNotNull(value: _turnDialNEView);

            _turnDialNEView.OnTurnDial += OnTurnDial;
            _turnDialNEView.OnSnapDial += OnSnapDial;
            _turnDialNEView.uiKnob.OnStartInteraction += OnStartDialInteraction;
            _turnDialNEView.uiKnob.OnStopInteraction += OnStopDialInteraction;
        }

        private void HideDial()
        {
            if (_turnDialNEView != null)
            {
                _turnDialNEView.OnTurnDial -= OnTurnDial;
                _turnDialNEView.OnSnapDial -= OnSnapDial;
                _turnDialNEView.uiKnob.OnStartInteraction -= OnStartDialInteraction;
                _turnDialNEView.uiKnob.OnStopInteraction -= OnStopDialInteraction;

                if (_context.ViewController.IsOverlayOpen<TurnDialNEView>())
                    _context.ViewController.CloseOverlay<TurnDialNEView>();

                _turnDialNEView = null;
            }
        }

        private void OnTurnDial(float dialValue)
        {
            _appManager.DialManager.SetNeedleRotation(dialValue);
        }
        private void OnSnapDial(int dialSnapValue)
        {
            //this is not wholly accurate - UIKnob needs to clamp its values i think before calculating the snap value
            //Debug.Log("SNAP " + dialSnapValue);

            ChangeEra(dialSnapValue);
        }

        private void OnStartDialInteraction()
        {
            //Stop the raycasts on the DialManager for as long as we are interacting
            //with the dial (it can't be hidden while we're using it.)
            _appManager.DialManager.ToggleRaycast(false);

        }
        private void OnStopDialInteraction()
        {
            //User has finished controlling the dial...
            //restart the dial manager raycasts which will close
            //the ui if we're not facing it.
            _appManager.DialManager.ToggleRaycast(true);
        }


        /// <summary>
        /// add some people to the scene and then assign each of them an AR Anchor
        /// </summary>
        private void AddSomePeople()
        {
            foreach (PeopleGroupInfo pgInfo in _appManager.peopleGroupsData.peopleGroups)
            {
                PeopleGroup peopleGroup = _appManager.PeopleGroupManager.AddPeopleGroup(pgInfo, null);

                //add an AR anchor for the group object
                _appManager.ARManager.AddARContent(peopleGroup.gameObject);
            }

            //Setup the subtitles camera (look-at) for one person in each group
            foreach (PeopleGroup pg in _appManager.PeopleGroupManager.PeopleGroups)
            {
                pg.People[0].SetupSubtitles(_appManager.ARManager.ARCamera);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapValue"></param>
        private void ChangeEra(int snapValue)
        {
            const int numEras = 5;
            if (snapValue >= numEras)
                return;

            Color[] eraColors = new Color[numEras] { Color.green, Color.red, Color.blue, Color.grey, Color.cyan};

            Color newColor = eraColors[snapValue];

            _appManager.PeopleGroupManager.umbrellaMaterial.SetColor("_Color",newColor);
        }
        #endregion

        #region RequiredMethods
        /// <summary>
        /// Begin is called once the state has been entered and the prefab has been instantiated.
        /// This is where you should initialize any runtime part of your state.
        /// </summary>
        public override void Begin()
        {
            _appManager = (AppManager)_context;
            Assert.IsNotNull(value: _appManager);

            _exploreNEView = (ExploreNEView)_context.ViewController.CurrentLocation;
            Assert.IsNotNull(value: _exploreNEView);

            _appManager.DialManager.OnFoundDial += ShowDial;
            _appManager.DialManager.OnLostDial += HideDial;

            _appManager.DialManager.Initialize(_appManager.ARManager.ARCamera);

            AddSomePeople();
        }

        /// <summary>
        /// Reason is called every frame without respect to delta time
        /// </summary>
        public override void Reason()
        {
        }

        /// <summary>
        /// Update is called every frame and gives you the delta time since last update
        /// </summary>
        /// <param name="deltaTime"></param>
        public override void Update(float deltaTime)
        {
            _appManager.DialManager.Tick();
    
#if UNITY_EDITOR
            //so we can test moving the camera off the thing in the editor while holding down the mouse click
            float debugCameraRotSpeed = 100f;
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Vector3 rot = _appManager.MainCamera.transform.localEulerAngles;
                rot.x += debugCameraRotSpeed * Time.deltaTime;
                _appManager.MainCamera.transform.localEulerAngles = rot;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Vector3 rot = _appManager.MainCamera.transform.localEulerAngles;
                rot.x -= debugCameraRotSpeed * Time.deltaTime;
                _appManager.MainCamera.transform.localEulerAngles = rot;
            }
#endif
        }

        /// <summary>
        /// End is called after the prefab has been destroyed, you should clean up anything else you have created in the
        /// scene / memory. The state instance itself lives on.
        /// </summary>
        public override void End()
        {
            _appManager.DialManager.OnFoundDial -= ShowDial;
            _appManager.DialManager.OnLostDial -= HideDial;

            HideDial();
        }
        #endregion
    }
}
