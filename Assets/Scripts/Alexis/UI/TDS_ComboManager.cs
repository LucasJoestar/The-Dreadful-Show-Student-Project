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
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private Animator comboManagerAnimator; 

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
        combocounter = 0;
        comboText.text = string.Empty; 
        resetComboCoroutine = null; 
    }
	#endregion

	#endregion
}
