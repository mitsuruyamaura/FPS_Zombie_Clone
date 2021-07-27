using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Weapon
    // 変数作成(スピーカー、音源)
    public AudioSource weapon;
    public AudioClip relodingSE, fireSE, triggerSE;

    // staticをつけてどこからでも呼び出せるようにする
    public static Weapon instance;

    // Weapon
    // 変数作成
    public Transform shotDirection;

    private void Awake()
    {
        // Weaponのinstanceの中が空なら自分自身を入れる
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(shotDirection.position, shotDirection.transform.forward * 10, Color.green);
    }

    public void CanShoot()
    {
        GameState.canShoot = true;
    }

    // 銃撃音とリロード音をつける(そのアニメーションでEvent作成)
    public void FireSE()
    {
        weapon.clip = fireSE;
        weapon.Play();
    }

    public void ReloadingSE()
    {
        weapon.clip = relodingSE;
        weapon.Play();
    }

    public void TriggerSE()
    {
        if (!weapon.isPlaying)
        {
            weapon.clip = triggerSE;
            weapon.Play();
        }
    }

    // 当たり判定のレーザーを飛ばす関数を作成
    public void Shooting()
    {
        RaycastHit hitInfo;

        // outをつけることで中身が空のものも引数として渡すことができる
        // 中身が空だけどこの先の処理で値が入るという宣言
        if (Physics.Raycast(shotDirection.transform.position, shotDirection.transform.forward, out hitInfo, 300))
        {
            // Weapon
            // ゾンビの死亡をアニメーションかラグドールのどちらかにする
            // GameObjectを格納する為の変数を作成
            // レーザーで何かゲームオブジェクトに当たった時にそれを格納する
            GameObject hitGameObject = hitInfo.collider.gameObject;

            if (hitInfo.collider.gameObject.GetComponent<ZombieController>() != null)
            {
                // ZimbieControllerを格納するために同じ型の変数を宣言する
                ZombieController hitZombie = hitInfo.collider.gameObject.GetComponent<ZombieController>();
                if (Random.Range(0, 10) < 5)
                {
                    // 一部のhitZombieをhitGameObjectに変更
                    hitZombie.ZombieDeath();

                    GameObject rgPrefab = hitZombie.ragdoll;

                    GameObject NewRD = Instantiate(rgPrefab, hitGameObject.transform.position, hitGameObject.transform.rotation);

                    NewRD.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(shotDirection.transform.forward * 5000);

                    // ぶつかったGameObjectを削除
                    Destroy(hitGameObject);
                }
                else
                {
                    hitZombie.ZombieDeath();
                }
            }
        }
    }
}
