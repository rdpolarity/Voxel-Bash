using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RDPolarity
{
    public class PlayerInfo : MonoBehaviour
    {
        [SerializeField] private Image playerIcon;
        [SerializeField] private List<Image> stocks;
        [SerializeField] private Image cross;
        
        private int stockCount;

        // Start is called before the first frame update
        void Start()
        {
            stockCount = stocks.Count-1;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void SetPlayerIcon(Sprite s)
        {
            playerIcon.sprite = s;
        }

        public void RemoveStock()
        {
            stocks[stockCount].gameObject.SetActive(false);
            stockCount--;
            if (stockCount < 0)
            {
                cross.gameObject.SetActive(true);
            }
        }

        public void UpdateInfo()
        {

        }
    }
}
