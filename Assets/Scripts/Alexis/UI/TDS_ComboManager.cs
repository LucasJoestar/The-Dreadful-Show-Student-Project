using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class TDS_ComboManager : MonoBehaviour 
{
    /* TDS_ComboManager :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	[TO DO]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[DATE]
	 *	Author :		[NAME]
	 *
	 *	Changes :
	 *
	 *	[CHANGES]
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    [SerializeField, Range(.1f, 5)] private float resetTimer = 2;
    [SerializeField, Range(0, 45)] private float limitRotation = 30; 
    [SerializeField] private TMP_Text comboText = null;
    [SerializeField] private Animator comboManagerAnimator = null; 

    private int combocounter = 0;
    private Coroutine resetComboCoroutine = null; 
	#endregion

	#region Methods

	#region Original Methods
    public void IncreaseCombo()
    {
        combocounter++;
        transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-limitRotation, limitRotation));
        if (comboManagerAnimator) comboManagerAnimator.SetTrigger("Increase"); 
        comboText.text = $"X {combocounter} !"; 
        if (resetComboCoroutine != null)
            StopCoroutine(resetComboCoroutine);
        resetComboCoroutine = StartCoroutine(ResetCombo()); 
    }

    private IEnumerator ResetCombo()
    {
        yield return new WaitForSeconds(resetTimer);
        ResetComboManager(); 
    }

    public void ResetComboManager()
    {
        combocounter = 0;
        comboText.text = string.Empty;
        resetComboCoroutine = null;
    }
    #endregion

    #region UnityMethod
    private void Start()
    {
        if (TDS_UIManager.Instance?.ComboManager == null)
            TDS_UIManager.Instance.ComboManager = this;
    }
    #endregion

    #endregion
}
