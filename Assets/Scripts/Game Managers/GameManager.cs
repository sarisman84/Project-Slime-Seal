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

        private Dictionary<Vector3, BallAffector> m_AllKnownAffectors;
        private List<BallAffector> m_LatestAffectors = new List<BallAffector>();

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
            m_AllKnownAffectors = new Dictionary<Vector3, BallAffector>();
            m_AllKnownAffectors = FindObjectsOfType<BallAffector>().Where(b =>
                    b.information.scaleType == BallAffectorInformation.ScaleType.ScaleUp)
                .ToDictionary(affector => affector.transform.position);
        }


        public void SetCheckpoint(GameObject checkPointPos)
        {
            m_LatestCheckpoint = checkPointPos.transform.position;
            m_LatestBallSize = m_Player.CurrentSize;
            m_LatestAffectors = FindObjectsOfType<BallAffector>()
                .Where(b => b.information.scaleType == BallAffectorInformation.ScaleType.ScaleUp &&
                            !b.GetComponent<Collider>().enabled).ToList();
        }

        public void ResetToCheckpoint()
        {
            m_Player.transform.position = m_LatestCheckpoint;
            m_Player.SetBallSize(m_LatestBallSize);
            m_Player.m_BallEnlarger.UpdateCaughtObjectsList(m_Player);
            foreach (KeyValuePair<Vector3, BallAffector> affector in m_AllKnownAffectors)
            {
                if (m_LatestAffectors.Contains(affector.Value))
                {
                    affector.Value.GetComponent<Collider>().enabled = true;
                    affector.Value.transform.position = affector.Key;
                    affector.Value.gameObject.layer = 9;
                    Destroy(affector.Value.GetComponent<Rigidbody>());
                    
                }
            }
        }

        public void ResetData()
        {
            m_LatestCheckpoint = Vector3.zero;
            m_LatestBallSize = 0;
            m_LatestAffectors = default;
            foreach (KeyValuePair<Vector3, BallAffector> affector in m_AllKnownAffectors)
            {
                affector.Value.GetComponent<Collider>().enabled = true;
                affector.Value.transform.position = affector.Key;
                Destroy(affector.Value.GetComponent<Rigidbody>());
            }
        }
    }
}