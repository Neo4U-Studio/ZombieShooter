using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuScene : MonoBehaviour
{
    public abstract MenuType GetMenuType();

    protected Animator m_Animator;

    protected bool isMenuShow = false;
    protected bool isInAnimation = false;

    public virtual void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public virtual void SetIsShown(bool value)
    {
        gameObject.SetActive(value);
        isMenuShow = value;
        if (isMenuShow)
            OnMenuEnabled();
    }

    public bool IsInAnimation()
    {
        return isInAnimation;
    }

    public IEnumerator OpenMenu()
    {
        while (isInAnimation) yield return null;

        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        if (m_Animator == null)
        {
            OnStartOpenMenu();
            OnFinishOpenMenu();

            isInAnimation = false;
        }
        else
        {
            m_Animator.SetTrigger("MenuShow");

            isInAnimation = true;
            OnStartOpenMenu();

            while (isInAnimation) yield return null;
        }
    }

    public IEnumerator CloseMenu()
    {
        while (isInAnimation) yield return null;

        if (m_Animator == null)
        {
            gameObject.SetActive(false);

            OnStartCloseMenu();
            OnFinishCloseMenu();

            isInAnimation = false;
        }
        else
        {
            m_Animator.SetTrigger("MenuHide");
            OnStartCloseMenu();

            isInAnimation = true;
            while (isInAnimation) yield return null;
        }
    }
    public virtual void SetInteractableButtons(bool isInteractable) {}
    public virtual void OnCreateMenu() {}

    public virtual void OnStartOpenMenu() {}

    public virtual void OnFinishOpenMenu()
    {
        isInAnimation = false;
        isMenuShow = true;
    }

    public virtual void OnStartCloseMenu()
    {
        isMenuShow = false;
    }

    public virtual void OnFinishCloseMenu()
    {
        gameObject.SetActive(false);
        isInAnimation = false;
    }

    public virtual void OnMenuPressed() { }

    public virtual void OnGamePaused() { }

    public virtual void OnGameResumed() { }

    public virtual void RefreshUI() { }

    public virtual void OnMenuEnabled() { }

}

