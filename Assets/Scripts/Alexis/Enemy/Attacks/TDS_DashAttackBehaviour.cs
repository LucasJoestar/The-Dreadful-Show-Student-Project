using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dash_Attack", menuName = "Attacks/Dash Attack", order = 5), Serializable]
public class TDS_DashAttackBehaviour : TDS_EnemyAttack
{
    [SerializeField] private bool isDashingForward = true;
    [SerializeField, Range(0, 25)] private float dashingDistance = 1;
    [SerializeField, Range(0.01f, 2)] private float dashingSpeed = 1;  


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
        Vector3 _originalPosition = _caster.transform.position;
        Vector3 _endPosition = _originalPosition; 
        if(isDashingForward)
        {
            _endPosition += (_caster.IsFacingRight ? _caster.transform.right : -_caster.transform.right) * dashingDistance; 
        }
        else
        {
            _endPosition -= (_caster.IsFacingRight ? _caster.transform.right : -_caster.transform.right) * dashingDistance;
        }
        while(_caster.IsAttacking)
        {
            _caster.transform.position = Vector3.MoveTowards(_caster.transform.position, _endPosition, Time.deltaTime * dashingSpeed * 10); 
            yield return null; 
        }
    }
}
