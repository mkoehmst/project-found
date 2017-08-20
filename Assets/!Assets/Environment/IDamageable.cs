namespace ProjectFound.Environment {

public interface IDamageable
{
	void TakeDamage( IDamageable attacker, float damage );
}

}