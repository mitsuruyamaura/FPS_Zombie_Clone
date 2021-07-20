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
                Debug.Log("弾が不足している");
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
            }
        }
        else if (animator.GetBool("Walk"))
        {
            animator.SetBool("Walk", false);
        }

        if (z > 0 && Input.GetKey(KeyCode.LeftShift))
        {
            if (!animator.GetBool("Run"))
            {
                animator.SetBool("Run", true);
                speed = 0.25f;
            }
        }
        else if (animator.GetBool("Run"))
        {
            animator.SetBool("Run", false);
            speed = 0.1f;
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
}
