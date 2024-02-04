using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private VisualElement _bottomContainer;
    private VisualElement _scrim;
    private VisualElement _bottomSheet;
    private VisualElement _boy;
    private VisualElement _girl;
    
    private Button _openButton;
    private Button _closeButton;

    private Label _message;

    void Start()
    {
        ElementBind();
        EventBind();
    }

    private void ElementBind()
    {
        var doc = GetComponent<UIDocument>().rootVisualElement;
        
        _bottomContainer = doc.Q<VisualElement>("container_bottom");
        _bottomSheet = doc.Q<VisualElement>("bottomsheet");
        _scrim = doc.Q<VisualElement>("scrim");
        _boy = doc.Q<VisualElement>("image_boy");
        _girl = doc.Q<VisualElement>("image_girl");

        _openButton = doc.Q<Button>("button_open");
        _closeButton = doc.Q<Button>("button_close");

        _message = doc.Q<Label>("message");
    }

    private void EventBind()
    {
        _openButton.RegisterCallback<ClickEvent>(OnOpenButtonClicked);
        _closeButton.RegisterCallback<ClickEvent>(OnCloseButtonClicked);

        _bottomContainer.style.display = DisplayStyle.None;

        _scrim.RemoveFromClassList("scrim--fadein");
        Invoke("AnimateBoy", 1.0f);
        
        _bottomSheet.RegisterCallback<TransitionEndEvent>(x =>
        {
            print(x.target);
            if (x.target != _bottomSheet) return;
            
            if(_bottomSheet.ClassListContains("bottomsheet--up") == false){
                
                _bottomContainer.style.display = DisplayStyle.None;
            }
        });
        _girl.RegisterCallback<TransitionEndEvent>(e =>
        {
            _girl.ToggleInClassList("image--girl--up");
        });
    }
    private void AnimateBoy()
    {
        _boy.RemoveFromClassList("image--boy-inair");
    }
    
    private void AnimateGirl()
    {
        _girl.ToggleInClassList("image--girl--up");

        _message.text = string.Empty;

        string m = "\"Sed in rebus apertissimis nimium longi sumus.\"";
        DOTween.To(() => _message.text, x => _message.text = x, m, 3f).SetEase(Ease.Linear);
    }

    private void OnOpenButtonClicked(ClickEvent e)
    {
        _bottomContainer.style.display = DisplayStyle.Flex;
        
        _bottomSheet.AddToClassList("bottomsheet--up");
        _scrim.AddToClassList("scrim--fadein");

        AnimateGirl();
    }

    private void OnCloseButtonClicked(ClickEvent e)
    {
        _bottomSheet.RemoveFromClassList("bottomsheet--up");
        _scrim.RemoveFromClassList("scrim--fadein");
    }
}