using UnityEngine;
using System.Collections;
using packt.FoodyGO.Services;
using packt.FoodyGO.Database;
using packt.FoodyGO.Managers;

namespace packt.FoodyGO.Controllers
{
    public class CatchSceneController : MonoBehaviour
    {
        public Transform frozenParticlePrefab;
        public MonsterController monster;
        public GameObject[] frozenDisableList;
        public GameObject[] frozenEnableList;

        public Transform escapeParticlePrefab;
        public float monsterChanceEscape;
        public float monsterWarmRate;
        public bool catching;
        public Monster monsterProps;

        void Start()
        {
            monsterProps = MonsterFactory.CreateRandomMonster();
            print(monsterProps);

            monsterChanceEscape = monsterProps.Power * monsterProps.Level;
            monsterWarmRate = .0001f * monsterProps.Power;
            catching = true;

            StartCoroutine(CheckEscape());
        }

        IEnumerator CheckEscape()
        {
            while (catching)
            {
                yield return new WaitForSeconds(30);
                if (Random.Range(0, 100) < monsterChanceEscape && monster!= null)
                {
                    catching = false;
                    print("Monster ESCAPED");
                    monster.gameObject.SetActive(false);
                    Instantiate(escapeParticlePrefab);
                    foreach (var g in frozenDisableList)
                    {
                        g.SetActive(false);
                    }
                }
            }
        }  

        public void OnMonsterHit(GameObject go, Collision collision)
        {
            monster = go.GetComponent<MonsterController>();
            if (monster != null)
            {
                monster.monsterWarmRate = monsterWarmRate;
                print("Monster hit");
                var animSpeedReduction = Mathf.Sqrt(collision.relativeVelocity.magnitude) / 10;
                monster.animationSpeed = Mathf.Clamp01(monster.animationSpeed - animSpeedReduction);
                if (monster.animationSpeed == 0)
                {
                    print("Monster FROZEN");
                    InventoryService.Instance.CreateMonster(monsterProps);


                    Instantiate(frozenParticlePrefab);

                    foreach(var g in frozenDisableList)
                    {
                        g.SetActive(false);
                    }
                    foreach(var g in frozenEnableList)
                    {
                        g.SetActive(true);
                    }
                }
            }
        }
    }
}
