using System;
using System.Collections.Generic;
using System.Linq;
using Interactivity;
using Player;
using UnityEngine;

namespace Game_Managers
{
    public class GameManager : MonoBehaviour
    {
        private BallController m_Player;
        private Vector3 m_LatestCheckpoint;
        private float m_LatestBallSize;

        private struct AffectorState
        {
            public AffectorState(Vector3 affectorPosition, bool affectorState, Quaternion affectorRotation,
                Transform affectorParent, bool objectCollisionState)
            {
                AffectorPosition = affectorPosition;
                ObjectState = affectorState;
                AffectorRotation = affectorRotation;
                AffectorParent = affectorParent;
                ObjectCollisionState = objectCollisionState;
            }

            public Transform AffectorParent;
            public Quaternion AffectorRotation;
            public Vector3 AffectorPosition;
            public bool ObjectState;
            public bool ObjectCollisionState;
        }

        private Dictionary<BridgeState, BridgeController> m_AllKnownBridges;
        private Dictionary<BridgeState, BridgeController> m_DefaultBridgeControllers;

        private Dictionary<AffectorState, BallAffector> m_DefaultAffectors;
        private Dictionary<AffectorState, BallAffector> m_AllKnownAffectors;
        private List<AffectorState> m_LatestAffectors = new List<AffectorState>();

        private static GameManager _ins;


        public static GameManager SingletonAccess
        {
            get
            {
                if (_ins == null)
                {
                    _ins =
                        FindObjectOfType<GameManager>() is { } gameManager
                            ? gameManager
                            : new GameObject("Game Manager").gameObject.AddComponent<GameManager>();
                }

                return _ins;
            }
        }

        private void Start()
        {
            m_Player = FindObjectOfType<BallController>();
            m_DefaultAffectors = new Dictionary<AffectorState, BallAffector>();
            m_AllKnownAffectors = new Dictionary<AffectorState, BallAffector>();
            m_AllKnownAffectors = (Dictionary<AffectorState, BallAffector>) FetchSceneObjects(WorldAsset.Affectors);

            m_AllKnownBridges = new Dictionary<BridgeState, BridgeController>();
            m_AllKnownBridges = (Dictionary<BridgeState, BridgeController>) FetchSceneObjects(WorldAsset.Bridges);


            m_DefaultAffectors = m_AllKnownAffectors;
            m_DefaultBridgeControllers = m_AllKnownBridges;
        }

        private object FetchSceneObjects(WorldAsset worldAsset)
        {
            switch (worldAsset)
            {
                case WorldAsset.Affectors:
                    List<BallAffector> foundObjs = FindObjectsOfType<BallAffector>().Where(b =>
                        b.information != null && b.information.scaleType == BallAffectorInformation.ScaleType.ScaleUp).ToList();


                    Dictionary<AffectorState, BallAffector> results = new Dictionary<AffectorState, BallAffector>();

                    AffectorState previousKey = default;
                    foreach (var obj in foundObjs)
                    {
                        var transform1 = obj.transform;
                        AffectorState key = new AffectorState(transform1.position, obj.IsPickedUpByPlayer,
                            transform1.rotation, transform1.parent, transform1.GetComponent<Collider>().enabled);
                        if (previousKey.Equals(key) || results.ContainsKey(key)) continue;
                        results.Add(key, obj);
                        previousKey = key;

                        Debug.Log($"Registered Ball Affector:{obj.name} to the Game Manager", obj);
                    }

                    return results;
                case WorldAsset.Bridges:
                    List<BridgeController> foundObjs1 = FindObjectsOfType<BridgeController>().ToList();


                    Dictionary<BridgeState, BridgeController>
                        results1 = new Dictionary<BridgeState, BridgeController>();

                    BridgeState previousKey1 = default;
                    foreach (var obj in foundObjs1)
                    {
                        var transform1 = obj.transform;
                        BridgeState key = new BridgeState(obj, obj.IsBridgeBuilt);
                        if (previousKey1.Equals(key) || results1.ContainsKey(key)) continue;
                        results1.Add(key, obj);
                        previousKey1 = key;

                        Debug.Log($"Registered Bridge Builder:{obj.name} to the Game Manager", obj);
                    }

                    return results1;
            }

            return default;
        }


        public void SetCheckpoint(GameObject checkPointPos)
        {
            m_LatestCheckpoint = checkPointPos.transform.position;
            m_LatestBallSize = m_Player.CurrentSize;
            m_AllKnownAffectors =
                (Dictionary<AffectorState, BallAffector>) UpdateWorldState(m_AllKnownAffectors, WorldAsset.Affectors);
            m_AllKnownBridges =
                (Dictionary<BridgeState, BridgeController>) UpdateWorldState(m_AllKnownBridges, WorldAsset.Bridges);
        }

        private object UpdateWorldState(
            object dictionary, WorldAsset worldAsset)
        {
            switch (worldAsset)
            {
                case WorldAsset.Affectors:
                    Dictionary<AffectorState, BallAffector> results = new Dictionary<AffectorState, BallAffector>();
                    foreach (var pair in (Dictionary<AffectorState, BallAffector>) dictionary)
                    {
                        var transform1 = pair.Value.transform;
                        results.Add(
                            new AffectorState(transform1.position, pair.Value.IsPickedUpByPlayer,
                                transform1.rotation, transform1.parent,
                                pair.Value.GetComponent<Collider>().enabled),
                            pair.Value);

                        //Debug.Log($"Saved Current state of {pair.Value.gameObject.name}");
                    }

                    return results;
                case WorldAsset.Bridges:
                    Dictionary<BridgeState, BridgeController> result1 = new Dictionary<BridgeState, BridgeController>();
                    foreach (var pair in (Dictionary<BridgeState, BridgeController>) dictionary)
                    {
                        result1.Add(
                            new BridgeState(pair.Value, pair.Value.IsBridgeBuilt),
                            pair.Value);

                        //Debug.Log($"Saved Current state of {pair.Value.gameObject.name}");
                    }

                    return result1;
            }

            return default;
        }


        public void ResetToCheckpoint()
        {
            Debug.Log("Resetting back to checkpoint");
            foreach (KeyValuePair<AffectorState, BallAffector> affector in m_AllKnownAffectors)
            {
                //Debug.Log(
                    //$"Current Saved State: IsPickedUp;{affector.Key.ObjectState},Parent:{affector.Key.AffectorParent.name},CollisionState:{affector.Key.ObjectCollisionState}");
                if (affector.Value.IsPickedUpByPlayer && affector.Key.ObjectState)
                {
                    // Debug.Break();
                    continue;
                }

                if (affector.Value.IsPickedUpByPlayer && !affector.Key.ObjectState)
                {
                    m_Player.m_BallEnlarger.ForceDropObject(affector.Value, affector.Key.AffectorPosition,
                        affector.Key.ObjectState, affector.Key.AffectorRotation, affector.Key.AffectorParent,
                        affector.Key.ObjectCollisionState);
                    Debug.Log($"Dropping  {affector.Value.gameObject.name}");
                }
                else if (!affector.Value.IsPickedUpByPlayer && affector.Key.ObjectState)
                {
                    m_Player.m_BallEnlarger.ForcePickupObject(affector.Value, affector.Key.ObjectState);
                    Debug.Log($"Picking up {affector.Value.gameObject.name}");
                }
            }

            foreach (KeyValuePair<BridgeState, BridgeController> bridgeController in m_AllKnownBridges)
            {
                if (!bridgeController.Key.IsBridgeBuilt && bridgeController.Value.IsBridgeBuilt)
                {
                    bridgeController.Value.ResetBridge();
                }
                else if (bridgeController.Key.IsBridgeBuilt && !bridgeController.Value.IsBridgeBuilt)
                {
                    bridgeController.Value.BuildBridge(m_Player.GetComponent<Collider>());
                }
            }

            m_Player.transform.position = m_LatestCheckpoint;
            m_Player.SetBallSize(m_LatestBallSize);
            // m_Player.m_BallEnlarger.UpdateCaughtObjectsList(m_Player);
        }

        public void ResetData()
        {
            m_LatestCheckpoint = Vector3.zero;
            m_LatestBallSize = 0;
            m_LatestAffectors = default;
            foreach (KeyValuePair<AffectorState, BallAffector> affector in m_AllKnownAffectors)
            {
                KeyValuePair<AffectorState, BallAffector> pair =
                    m_DefaultAffectors.First(p => p.Key.Equals(affector.Key));
                if (pair.Key.ObjectState)
                    m_Player.m_BallEnlarger.ForceDropObject(affector.Value, affector.Key.AffectorPosition,
                        affector.Key.ObjectState, affector.Key.AffectorRotation, affector.Key.AffectorParent,
                        affector.Key.ObjectCollisionState);
            }

            foreach (var bridgeController in m_AllKnownBridges)
            {
                var pair = m_DefaultBridgeControllers.First(b => b.Key.Equals(bridgeController.Key));

                if (pair.Key.IsBridgeBuilt)
                    pair.Value.ResetBridge();
            }

            m_Player.m_BallEnlarger.ForceDropAllObjects();
            ResetToCheckpoint();
        }
    }

    public enum WorldAsset
    {
        Affectors,
        Bridges
    }

    internal struct BridgeState
    {
        public BridgeState(BridgeController controller, bool isBridgeBuilt)
        {
            Controller = controller;
            IsBridgeBuilt = isBridgeBuilt;
        }

        public BridgeController Controller;
        public bool IsBridgeBuilt;
    }
}