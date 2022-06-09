using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Kakera
{
    public class PickerController : MonoBehaviour
    {
        [SerializeField]
        private Unimgpicker imagePicker;
        public GameObject coverPost;

        [SerializeField]
        private MeshRenderer imageRenderer;
        [SerializeField]
        private RawImage rawImage;
        public List<float> postSizes;
        private int[] sizes = {1024, 256, 16};

        void Awake()
        {
            imagePicker.Completed += (string path) =>
            {
                //StartCoroutine(LoadImageToCube(path, imageRenderer));
                StartCoroutine(loadImageToRawImage(path, rawImage));
            };
        }

        public void OnPressShowPicker()
        {
            imagePicker.Show("Select Image", "unimgpicker");
        }

        private IEnumerator LoadImageToCube(string path, MeshRenderer output)
        {
            var url = "file://" + path;
            var unityWebRequestTexture = UnityWebRequestTexture.GetTexture(url);
            yield return unityWebRequestTexture.SendWebRequest();

            var texture = ((DownloadHandlerTexture)unityWebRequestTexture.downloadHandler).texture;
            if (texture == null)
            {
                Debug.LogError("Failed to load texture url:" + url);
            }

            output.material.mainTexture = texture;
        }
        private IEnumerator loadImageToRawImage(string path, RawImage output)
        {
            var url = "file://" + path;
            var unityWebRequestTexture = UnityWebRequestTexture.GetTexture(url);
            yield return unityWebRequestTexture.SendWebRequest();

            var texture = ((DownloadHandlerTexture)unityWebRequestTexture.downloadHandler).texture;
            float width = texture.width;
            float height = texture.height;
            if (texture == null)
            {
                Debug.LogError("Failed to load texture url:" + url);
            }
            float resPost = postSizes[0] / postSizes[1];
            float resImg = width / height;
            if(resImg <= resPost)
            {
                float factor = 540 / width;
                width *= factor;
                height *= factor;
            }
            else
            {
                float factor = 540 / height;
                width *= factor;
                height *= factor;
            }
            output.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            output.texture = texture;
            coverPost.SetActive(true);
        }
    }
}