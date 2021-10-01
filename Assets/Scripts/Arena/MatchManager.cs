using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
namespace RDPolarity
{
    public class MatchManager : NetworkBehaviour
    {
        [SerializeField] private TMP_Text timer;
        [SyncVar(hook = nameof(UpdateTimer))] private float roundTime;

        // Start is called before the first frame update
        void Start()
        {
            roundTime = 300;
        }

        private void Update()
        {
            roundTime -= Time.deltaTime;
        }

        // Update is called once per frame
        private void UpdateTimer(float oldValue, float newValue)
        {
            timer.text = ((int)roundTime).ToString();
        }


    }
}
