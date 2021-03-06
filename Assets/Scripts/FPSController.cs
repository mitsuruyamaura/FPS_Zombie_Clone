using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSController : MonoBehaviour
{
    // 移動用の変数を作成
    float x, z;

    // スピード調整用の変数を作成
    float speed = 0.1f;

    // 変数の宣言
    public GameObject cam;

    Quaternion cameraRot, characterRot;
    float Xsensityvity = 3f, Ysensityvity = 3f;

    // 変数の宣言
    bool cursorLock = true;

    // 変数の宣言(角度の制限用）
    float minX = -90f, maxX = 90f;

    // 変数の宣言(アニメーション用)
    public Animator animator;

    // 変数の宣言(所持弾薬、最高所持弾薬、マガジンないの弾数、マガジン内の最大数)
    int ammunition = 50, maxAmmunition = 50, ammoClip = 10, maxAmmoClip =10;

    // 変数の宣言(体力、Max体力、体力バー、弾薬テキスト)
    int playerHP = 100, maxPlayerHP = 100;
    public Slider hpBer;
    public Text ammoText;

    // 変数(カメラの取得)
    public GameObject meinCamera, subCamera;

    // FPSController
    // 変数作成(スピーカー、音源)
    public AudioSource playerFootStep;
    public AudioClip WalkFootStepSE, RunFootStepSE;

    // FPSController
    // 変数を作成(スピーカー、音)
    public AudioSource voise,  impact;
    public AudioClip hitVoiseSE, HitImpactSE;

    

    // 作成した関数を呼ぶ(攻撃食らった時に呼ぶ関数)


    // スタート時に体力を体力バーに反映
    // リロードと弾薬の所でテキストを反映する
    void Start()
    {
        cameraRot = cam.transform.localRotation;
        characterRot = transform.localRotation;

        GameState.canShoot = true;

        hpBer.value = playerHP;
        ammoText.text = ammoClip + "/" + ammunition;
    }

    // アップデートでマウスの入力を受け取り、その動きをカメラに反映
    // アップデートで各ボタンの入力を確認したらアニメーション遷移
    void Update()
    {
        float xRot = Input.GetAxis("Mouse X") * Ysensityvity;
        float yRot = Input.GetAxis("Mouse Y") * Xsensityvity;

        cameraRot *= Quaternion.Euler(-yRot, 0, 0);
        characterRot *= Quaternion.Euler(0, xRot, 0);

        cameraRot = ClampRotation(cameraRot);

        cam.transform.localRotation = cameraRot;
        transform.localRotation = characterRot;

        // 作成した関数をUpdateで呼び出す
        UpdeateCursorLock ();

        // 射撃・リロード・歩き・走り
        // 動画6で条件追加
        if (Input.GetMouseButton(0) && GameState.canShoot)
        {
            // 射撃とリロードの所にコード追加
            // リロードと弾薬の所でテキストを反映する
            if (ammoClip > 0)
            {
                animator.SetTrigger("Fire");
                GameState.canShoot = false;

                ammoClip--;
                ammoText.text = ammoClip + "/" + ammunition;
            }
            else
            {
                //Debug.Log("弾が不足している");

                Weapon.instance.TriggerSE();
            }
        }

        // リロードと弾薬の所でテキストを反映する
        if (Input.GetKeyDown(KeyCode.R))
        {
            int amountNeed = maxAmmoClip - ammoClip;
            int ammoAvailable = amountNeed < ammunition ? amountNeed : ammunition;

            if (amountNeed != 0 && ammunition != 0)
            {
                animator.SetTrigger("Reload");

                ammunition -= ammoAvailable;
                ammoClip += ammoAvailable;
                ammoText.text = ammoClip + "/" + ammunition;
            }

            
        }

        if (Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0)
        {
            if (!animator.GetBool("Walk"))
            {
                animator.SetBool("Walk", true);

                // 適切な場所で呼び出す
                PlayerWalkFootStep(WalkFootStepSE);
            }
        }
        else if (animator.GetBool("Walk"))
        {
            animator.SetBool("Walk", false);

            StopFootStep();
        }

        if (z > 0 && Input.GetKey(KeyCode.LeftShift))
        {
            if (!animator.GetBool("Run"))
            {
                animator.SetBool("Run", true);
                speed = 0.25f;

                // 適切な場所で呼び出す
                PlayerRunFootStep(RunFootStepSE);
            }
        }
        else if (animator.GetBool("Run"))
        {
            animator.SetBool("Run", false);
            speed = 0.1f;

            StopFootStep();
        }

        // アップデートで右クリック検知してカメラを切り替える
        if (Input.GetMouseButton(1))
        {
            subCamera.SetActive(true);
            meinCamera.GetComponent<Camera>().enabled = false;
        }
        else if (subCamera.activeSelf)
        {
            subCamera.SetActive(false);
            meinCamera.GetComponent<Camera>().enabled = true;
        }
    }

    // 入力に合わせてプレイヤーの位置を変更していく
    // カメラの正面方向に進むようにコード記述
    private void FixedUpdate()
    {
        x = 0;
        z = 0;

        x = Input.GetAxisRaw("Horizontal")　* speed;
        z = Input.GetAxisRaw("Vertical") * speed;

        //transform.position += new Vector3(x, 0, z);

        transform.position += cam.transform.forward * z + cam.transform.right * x;
    }

    // マウスカーソルの表示を切り替える関数を作成する
    public void UpdeateCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLock = false;
        }
        else if (Input.GetMouseButton(0))
        {
            cursorLock = true;
        }

        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (!cursorLock)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    // 角度制限関数の作成
    public Quaternion ClampRotation(Quaternion q)
    {
        // q = x, y, z w (x, y, zはベクトル (量と向き)　: wはスカラー (座標とは無関係の量 : 回転する))
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;

        angleX = Mathf.Clamp(angleX, minX, maxX);

        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);

        return q;
    }

    // 実際に音を出す関数の作成
    public void PlayerWalkFootStep(AudioClip clip)
    {
        playerFootStep.loop = true;

        playerFootStep.pitch = 1f;

        playerFootStep.clip = clip;

        playerFootStep.Play();
    }

    // 実際に音を出す関数の作成
    public void PlayerRunFootStep(AudioClip clip)
    {
        playerFootStep.loop = true;

        playerFootStep.pitch = 1.3f;

        playerFootStep.clip = clip;

        playerFootStep.Play();
    }

    // 実際に音を出す関数の作成
    public void StopFootStep()
    {
        playerFootStep.Stop();

        playerFootStep.loop = false;

        playerFootStep.pitch = 1f;
    }

    // FPSController
    // HPを減らす関数を作成する
    public void TakeHit(float damage)
    {
        // 何もしないと左と右で型が違うためエラーが出るため
        // playerHPがint型なので、Mathfの前に(int)とすることでint型にできる
        playerHP = (int)Mathf.Clamp(playerHP - damage, 0, playerHP);

        hpBer.value = playerHP;

        ImpactSE();

        if (Random.Range(0, 10) < 6)
        {
            VoiseSE(hitVoiseSE);
        }

        if (playerHP <= 0 && !GameState.GameOver)
        {
            GameState.GameOver = true;
        }
    }

    // 関数作成(攻撃を食らったときの音、声)
    public void VoiseSE(AudioClip clip)
    {
        voise.Stop();

        voise.clip = clip;
        voise.Play();
    }

    public void ImpactSE()
    {
        voise.clip = HitImpactSE;
        voise.Play();
    }

}
