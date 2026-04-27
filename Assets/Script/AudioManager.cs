using UnityEngine;

public class DontDestroyAudio : MonoBehaviour
{
    private static DontDestroyAudio instance;

    void Awake()
    {
        // ตรวจสอบว่ามี AudioManager อยู่แล้วหรือยัง (ป้องกันเพลงเล่นซ้อนกัน)
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ทำให้วัตถุนี้ไม่ถูกลบเมื่อเปลี่ยน Scene
        }
        else
        {
            Destroy(gameObject); // ถ้ามีอยู่แล้วให้ลบตัวที่สร้างใหม่ทิ้ง
        }
    }
}