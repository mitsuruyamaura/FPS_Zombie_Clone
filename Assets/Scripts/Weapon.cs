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
}
