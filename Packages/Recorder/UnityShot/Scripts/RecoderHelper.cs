using System;
using System.IO;
using UnityEngine;

namespace ZKnight.VideoRecorder.Runtime
{
    public class RecoderHelper : MonoBehaviour
    {
        [NonSerialized]
        public Camera Camera;
        [NonSerialized]
        public GameObject Player;
        public float Distance = 10f;
        public float Angle = 30f;
        public float MaxTime = 5f;

        private GameObject _tempObject;
        private GameObject _shotCamera;

        public string SrcPath;

        public void Start()
        {
            Player = gameObject;
            Camera = Camera.main;

            _tempObject = new GameObject();
            _tempObject.transform.position = transform.position;
            _tempObject.transform.rotation = transform.rotation;
            _tempObject.transform.localScale = transform.lossyScale;
            _tempObject.hideFlags = HideFlags.DontSave;

            Camera camera = Camera;
            Camera.transform.SetParent(_tempObject.transform);
            float deltaY = Mathf.Sin(Angle * Mathf.Deg2Rad) * Distance;
            float deltaZ = Mathf.Cos(Angle * Mathf.Deg2Rad) * Distance;

            Vector3 positionWS = Player.transform.TransformPoint(new Vector3(0, deltaY, deltaZ));
            camera.transform.position = positionWS;
            camera.transform.LookAt(Player.transform);

            TakeShot();
        }

        public void Update()
        {
            if (Time.time > MaxTime)
            {
                return;
            }

            float lerp = Time.time / MaxTime;
            float angleY = 360f * lerp;
            Vector3 euler = _tempObject.transform.eulerAngles;
            euler.y = angleY;
            _tempObject.transform.eulerAngles = euler;
        }

        private void TakeShot()
        {
            _shotCamera = Instantiate(_tempObject);
            Camera camera = _shotCamera.GetComponentInChildren<Camera>();
            camera.transform.localPosition = new Vector3(0, 0, -Distance);
            camera.transform.LookAt(camera.transform.parent);
            RenderTexture texture = RenderTexture.GetTemporary(Screen.width, Screen.height);
            camera.targetTexture = texture;

            RunShotAndWrite(camera, texture, $"{SrcPath}\\front.png");
            _shotCamera.transform.Rotate(_shotCamera.transform.up, 90f);
            RunShotAndWrite(camera, texture, $"{SrcPath}\\Left.png");
            _shotCamera.transform.Rotate(_shotCamera.transform.up, 90f);
            RunShotAndWrite(camera, texture, $"{SrcPath}\\Back.png");

            camera.targetTexture = null;
            RenderTexture.ReleaseTemporary(texture);
            Destroy(_shotCamera);
        }

        private void RunShotAndWrite(Camera camera, RenderTexture texture, string path)
        {
            camera.Render();
            using (FileStream file = new FileStream(path, FileMode.Create))
            {
                byte[] bytes = ImageBytes(texture);
                file.Write(bytes, 0, bytes.Length);
            }
        }

        private byte[] ImageBytes(RenderTexture tex)
        {
            Texture2D tex2D = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
            RenderTexture.active = tex;
            tex2D.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            tex2D.Apply();
            return tex2D.EncodeToPNG();
        }

        public void OnDestroy()
        {
            Destroy(_tempObject);
        }
    }
}
