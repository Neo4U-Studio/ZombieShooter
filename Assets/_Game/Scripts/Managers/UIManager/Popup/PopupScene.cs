using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PopupScene : MonoBehaviour
{
    public abstract PopupType GetPopupType();

    protected Animator m_Animator;

    public bool IsPopupShow() { return isPopupShow; }
    protected bool isPopupShow = false;
    protected bool isInAnimation = false;

    public virtual void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void SetIsShown(bool value)
    {
        isPopupShow = value;
        if (isPopupShow)
            OnPopupEnabled();
    }

    public bool IsInAnimation()
    {
        return isInAnimation;
    }

    public IEnumerator OpenPopup()
    {
        while (isInAnimation) yield return null;

        if (m_Animator == null)
        {
            OnStartOpenPopup();
            OnFinishOpenPopup();

            isInAnimation = false;
        }
        else
        {
            m_Animator.SetTrigger("PopupShow");

            isInAnimation = true;
            OnStartOpenPopup();

            while (isInAnimation) yield return null;
        }
    }
    public IEnumerator ClosePopup()
    {
        while (isInAnimation) yield return null;

        if (m_Animator == null)
        {
            gameObject.SetActive(false);

            OnStartClosePopup();
            OnFinishClosePopup();

            isInAnimation = false;
        }
        else
        {
            m_Animator.SetTrigger("PopupHide");
            OnStartClosePopup();

            isInAnimation = true;
            while (isInAnimation) yield return null;
        }
    }

    public virtual void OnCreatePopup()
    {
    }

    public virtual void OnDestroyPopup()
    {
    }

    public virtual void OnStartOpenPopup()
    {
    }

    public virtual void OnFinishOpenPopup()
    {
        isInAnimation = false;
        isPopupShow = true;
    }

    public virtual void OnStartClosePopup()
    {
        isPopupShow = false;
    }

    public virtual void OnFinishClosePopup()
    {
        gameObject.SetActive(false);
        isInAnimation = false;
    }

    public virtual void OnBackPressed() { }

    public virtual void OnPopupPressed() { }

    public virtual void OnGamePaused() { }

    public virtual void OnGameResumed() { }

    public virtual void RefreshUI() { }

    public virtual void OnPopupEnabled() { }
}
