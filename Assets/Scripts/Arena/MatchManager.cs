using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RDPolarity.UI;
using UnityEngine.UI;


namespace RDPolarity
{
    public class MatchManager : NetworkBehaviour
    {
        [SerializeField] private UIController uiController;
        [SerializeField] private bool suddenDeath;
        [SyncVar(hook = nameof(UpdateTimer))] private float roundTime;

        private Dictionary<GameObject, PlayerInfo> players = new Dictionary<GameObject, PlayerInfo>();
        [SerializeField] private GameObject playerInfoPrefab;
        [SerializeField] private List<Vector2> infoSpawnPoints;

        // Start is called before the first frame update
        void Start()
        {
            roundTime = 10;
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (g.name.Contains("Player"))
                {
                    AddPlayer(g);
                }
            }
        }

        private void Update()
        {
            roundTime -= Time.deltaTime;

            if (roundTime < 0)
            {
                suddenDeath = true;
            }
        }

        // Update is called once per frame
        private void UpdateTimer(float oldValue, float newValue)
        {
            if (suddenDeath)
            {
                uiController.UpdateTimer("Sudden Death");
            }
            else
            {
                uiController.UpdateTimer(((int)roundTime).ToString());
            }
            
        }

        private void AddPlayer(GameObject player)
        {
            
            GameObject temp = Instantiate(playerInfoPrefab);
            temp.transform.SetParent(uiController.transform);
            temp.GetComponent<RectTransform>().position = infoSpawnPoints[players.Count];
            players[player] = temp.GetComponent<PlayerInfo>();
        }

        private void UpdatePlayerInfos(GameObject player)
        {
            players[player].UpdateInfo();
        }
    }
}
