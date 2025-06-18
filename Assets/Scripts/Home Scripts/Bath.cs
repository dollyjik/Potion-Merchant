using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bath : MonoBehaviour
{
    public GameObject karartmaPaneli; // Canvas üzerindeki panel objesi
        public float kararmaSuresi = 1.5f; // Ekranın tamamen kararması için geçen süre
        public float beklemeSuresi = 2.0f; // Ekran karardıktan sonra beklenecek süre
        public float acilmaSuresi = 1.5f; // Ekranın tekrar açılması için geçen süre
    
        private Image panelImage;
    
        void Start()
        {
            if (karartmaPaneli != null)
            {
                panelImage = karartmaPaneli.GetComponent<Image>();
                if (panelImage == null)
                {
                    Debug.LogError("Karartma Paneli üzerinde Image bileşeni bulunamadı!");
                    return;
                }
                Color currentColor = panelImage.color;
                panelImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);
                karartmaPaneli.SetActive(false); // Paneli aktif tut ama şeffaf olsun
            }
            else
            {
                Debug.LogError("Karartma Paneli atanmamış! Lütfen Inspector'dan atayın.");
            }
        }
    
        public void TetikleEkranKarartma()
        {
            if (karartmaPaneli != null && panelImage != null)
            {
                StopAllCoroutines();
                karartmaPaneli.SetActive(true);
                StartCoroutine(EkranKarartVeAc());
            }
        }
    
        IEnumerator EkranKarartVeAc()
        {
            // Ekranı karartma
            float timer = 0f;
            Color startColor = panelImage.color;
            Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f);
    
            while (timer < kararmaSuresi)
            {
                timer += Time.deltaTime;
                panelImage.color = Color.Lerp(startColor, targetColor, timer / kararmaSuresi);
                yield return null;
            }
            panelImage.color = targetColor;
    
            yield return new WaitForSeconds(beklemeSuresi);
    
            timer = 0f;
            startColor = panelImage.color; 
            targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f); 
    
            while (timer < acilmaSuresi)
            {
                timer += Time.deltaTime;
                panelImage.color = Color.Lerp(startColor, targetColor, timer / acilmaSuresi);
                yield return null;
            }
            panelImage.color = targetColor; 
        }
}
