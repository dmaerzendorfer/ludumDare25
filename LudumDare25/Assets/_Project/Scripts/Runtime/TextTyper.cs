using System.Collections;
using _Project.Scripts.Runtime.Feedback;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Runtime
{
    public class TextTyper : MonoBehaviour
    {
        public TextMeshProUGUI textField;
        public float timeForLetter = 0.05f;
        public RandomAudioSourceFeedback letterFeedback;

        public UnityEvent onTypingComplete = new UnityEvent();

        private Coroutine _routine;
        private string _fullText;

        /// <summary>
        /// returns the duration it needs
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public float TypeOutText(string text)
        {
            _fullText = text;
            if (_routine != null)
                StopCoroutine(_routine);

            _routine = StartCoroutine(TypeText());
            return text.Length * timeForLetter;
        }

        private IEnumerator TypeText()
        {
            textField.text = ""; // Clear text
            foreach (char letter in _fullText)
            {
                textField.text += letter; // Add one letter at a time
                letterFeedback.Play();
                yield return new WaitForSeconds(timeForLetter);
            }

            onTypingComplete.Invoke();
        }
    }
}