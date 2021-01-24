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

        private Dictionary<AffectorState, BallAffector> m_defaultAffectors;
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

        private void Awake()
        {
            m_Player = FindObjectOfType<BallController>();
            m_defaultAffectors = new Dictionary<AffectorState, BallAffector>();
            m_AllKnownAffectors = new Dictionary<AffectorState, BallAffector>();
            m_AllKnownAffectors = FetchSceneObjects();


            m_defaultAffectors = m_AllKnownAffectors;
        }

        private Dictionary<AffectorState, BallAffector> FetchSceneObjects()
        {
            List<BallAffector> foundObjs = FindObjectsOfType<BallAffector>().Where(b =>
                b.information.scaleType == BallAffectorInformation.ScaleType.ScaleUp).ToList();


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
        }


        public void SetCheckpoint(GameObject checkPointPos)
        {
            m_LatestCheckpoint = checkPointPos.transform.position;
            m_LatestBallSize = m_Player.CurrentSize;
            m_AllKnownAffectors = UpdateWorldState(m_AllKnownAffectors);
        }

        private Dictionary<AffectorState, BallAffector> UpdateWorldState(
            Dictionary<AffectorState, BallAffector> allKnownAffectors)
        {
            Dictionary<AffectorState, BallAffector> results = new Dictionary<AffectorState, BallAffector>();
            foreach (var pair in allKnownAffectors)
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
        }


        public void ResetToCheckpoint()
        {
            Debug.Log("Resetting back to checkpoint");
            foreach (KeyValuePair<AffectorState, BallAffector> affector in m_AllKnownAffectors)
            {
                Debug.Log(
                    $"Current Saved State: IsPickedUp;{affector.Key.ObjectState},Parent:{affector.Key.AffectorParent.name},CollisionState:{affector.Key.ObjectCollisionState}");
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
                    m_defaultAffectors.First(p => p.Key.Equals(affector.Key));
                if (pair.Key.ObjectState)
                    m_Player.m_BallEnlarger.ForceDropObject(affector.Value, affector.Key.AffectorPosition,
                        affector.Key.ObjectState, affector.Key.AffectorRotation, affector.Key.AffectorParent,
                        affector.Key.ObjectCollisionState);
            }
        }
    }
}