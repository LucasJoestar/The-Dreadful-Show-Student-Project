using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dash_Attack", menuName = "Attacks/Dash Attack", order = 5), Serializable]
public class TDS_DashAttackBehaviour : TDS_EnemyAttack
{
    [SerializeField] private bool isDashingForward = true;
    [SerializeField, Range(0, 25)] private float dashingDistance = 1;
    [SerializeField, Range(0.01f, 5)] private float dashingSpeed = 1;  


    public override void ApplyAttackBehaviour(TDS_Enemy _caster)
    {
        if (!PhotonNetwork.isMasterClient) return; 
        if (_caster.Agent.IsMoving)
            _caster.Agent.StopAgent();
        _caster.StartCoroutine(DashingCoroutine(_caster)); 
    }

    private IEnumerator DashingCoroutine(TDS_Enemy _caster)
    {
        if (!PhotonNetwork.isMasterClient) yield break;
        _caster.GetComponent<Animator>().ResetTrigger("StopDashing");
        Vector3 _originalPosition = _caster.transform.position;
        Vector3 _endPosition = _originalPosition; 
        if(isDashingForward)
        {
            _endPosition += (_caster.IsFacingRight ? Vector3.right : -Vector3.right) * dashingDistance; 
        }
        else
        {
            _endPosition -= (_caster.IsFacingRight ? Vector3.right : -Vector3.right) * dashingDistance;
        }
        _endPosition.x = Mathf.Clamp(_endPosition.x, TDS_Camera.Instance.CurrentBounds.XMin + _caster.Agent.Radius, TDS_Camera.Instance.CurrentBounds.XMax - _caster.Agent.Radius);
        if (_caster is TDS_MightyMan _mightyManIn) _mightyManIn.IsDashing = true;
        while (_caster.IsAttacking && Vector3.Distance(_caster.transform.position, _endPosition) > 0)
        {
            _caster.transform.position = Vector3.MoveTowards(_caster.transform.position, _endPosition, Time.deltaTime * dashingSpeed * 10); 
            yield return null; 
        }
        if (_caster is TDS_MightyMan _mightyManOut) _mightyManOut.IsDashing = false;
        _caster.SetAnimationState((int)EnemyAnimationState.StopDashing);
        if (_caster.IsAttacking) _caster.StopAttack(); 
    }
}
