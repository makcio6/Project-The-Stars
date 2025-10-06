using UnityEngine;
using Crawler.Utils;

public class Enemy1Visual : MonoBehaviour
{
    public Animator animator;
    private Vector2 movement;
    private Vector2 lastDirection;
    private EnemyAI enemyAI;

    private bool isAttacking;
    private Vector2 attackDirection;

    [Header("Attack Points")]   //места где будет появляться хитбокс, привязанные к соответствующим пустым объектам возле моба
    public Transform upAttackPoint;
    public Transform downAttackPoint;
    public Transform leftAttackPoint;
    public Transform rightAttackPoint;
    public Transform upLeftAttackPoint;
    public Transform upRightAttackPoint;
    public Transform downLeftAttackPoint;
    public Transform downRightAttackPoint;

    [Header("Hitbox")]
    public GameObject attackHitbox; //привязка объекта самого хитбокса

    [ContextMenu("Test StartAttack")] //тестовый вызов метода атаки
    private void DebugStartAttack()
    {
        Debug.Log("Called StartAttack()");
        StartAttack();
    }


    private void Awake()
    {
        enemyAI = GetComponentInParent<EnemyAI>(); //вызов компонента в родительском объекте ИИ моба
        animator = GetComponent<Animator>(); //вызов компонента аниматора
        lastDirection = Vector2.down;  //стандартное направление моба для отображения при спавне
    }

    private void Update()
    {
        if (isAttacking || enemyAI.IsAttacking()) return; //если в данный момент вызывается метод атаки, метод Update работать не должен

        Vector3 velocity = enemyAI.NavMeshAgent.velocity; //декларование инерции из навАгента как переменной
        movement = new Vector2(velocity.x, velocity.y); //декларирование переменной передвижения

        if (movement.sqrMagnitude > 0.01f) //проверка движетсяли объект
        {
            float horizontal = Mathf.Sign(movement.x); //определение движения по горизонтали
            float vertical = Mathf.Sign(movement.y); //определение движения по вертикали
            lastDirection = new Vector2(horizontal, vertical); //сохранение нормализованного последнего направления 
        }

        UpdateAnimation(); //вызов метода после всех предыдущих действий
    }

    public void UpdateAnimation()
    {
        if (isAttacking || enemyAI.IsAttacking()) return;

        float horizontal, vertical; 

        if (movement.sqrMagnitude > 0.01f) //проверка присутствия движения
        {
            Vector2 normalizedMovement = movement.normalized; //декларирование переменной нормализованного движения

            horizontal = Mathf.Round(normalizedMovement.x); //округление нормализованного движения по горизонтали
            vertical = Mathf.Round(normalizedMovement.y); //округление нормализованного движения по вертикали

            if (horizontal != 0 || vertical != 0) //проверка если движение по горизонтали или вертикали не равно нулю
            {
                lastDirection = new Vector2(horizontal, vertical); //постоянное обновление переменных последнего направления
            }

            animator.SetBool("IsMoving", true); //изменение флага движения в аниматоре на true
        }
        else //если же движения нет, устанавливает последнее направление и меняет флаг движения на false
        {
            horizontal = lastDirection.x;
            vertical = lastDirection.y;
            animator.SetBool("IsMoving", false);
        }

        //ниже изменение переменных движения и последнего направления в аниматоре, и отладка в консоль
        animator.SetFloat("Horizontal", horizontal); 
        animator.SetFloat("Vertical", vertical);

        animator.SetInteger("LastHorizontal", (int)horizontal);
        animator.SetInteger("LastVertical", (int)vertical);

        Debug.Log($"Idle check: LastH={animator.GetInteger("LastHorizontal")}, LastV={animator.GetInteger("LastVertical")}, IsMoving={animator.GetBool("IsMoving")}");
    }


    public void SetAttackDirection(Vector2 dir) //вызов атаки только в последнее направление куда был повёрнут моб
    {
        attackDirection = dir;
        lastDirection = dir;
        animator.SetInteger("LastHorizontal", (int)dir.x);
        animator.SetInteger("LastVertical", (int)dir.y);
    }

    public void StartAttack() //вызывается метод через анимацию, ставит ИИ в состояние атаки, и устанавливает триггер в аниматоре для проигрывания анимации атаки
    {
        isAttacking = true;
        Debug.Log("Attack trigger set");
        animator.SetTrigger("Attack");
    }

    public void OnAttackAnimationEnd() //вызывается в конце анимации, выключает состояние атаки, и вызывает метод окончания атаки в ИИ моба
    {
        isAttacking = false;

        enemyAI.EndAttack();
    }

    public void SpawnHitbox() 
    {
        if (enemyAI.attackHitbox == null) return; //не будет работать если хитбокс атаки не назначен

        Transform attackPoint = GetCurrentAttackPoint(); //получение точки спавна хитбокса
        GameObject hitbox = Instantiate(enemyAI.attackHitbox, attackPoint.position, Quaternion.identity); //создание хитбокса атаки как копию уже созданного префаба на определённой позиции
        hitbox.transform.rotation = Quaternion.identity; //сброс вращения объекта хитбокса
    }

    public Transform GetCurrentAttackPoint() //настройка позиции спавна хитбокса
    {
        float x = animator.GetInteger("LastHorizontal");
        float y = animator.GetInteger("LastVertical");

        Debug.Log($"Spawning hitbox at direction X:{x} Y:{y}");
        //ниже координаты которые определяют точку из восьми доступных где он должен появиться
        if (x == 0 && y > 0) return upAttackPoint;
        if (x == 0 && y < 0) return downAttackPoint;
        if (x > 0 && y == 0) return rightAttackPoint;
        if (x < 0 && y == 0) return leftAttackPoint;
        if (x > 0 && y > 0) return upRightAttackPoint;
        if (x < 0 && y > 0) return upLeftAttackPoint;
        if (x > 0 && y < 0) return downRightAttackPoint;
        if (x < 0 && y < 0) return downLeftAttackPoint;

        return downAttackPoint;
    }

    public void UpdateLookDirection(Vector2 direction) //обновление точки куда должен смотреть моб
    {
        if (direction.sqrMagnitude < 0.01f) //проверка отсутствия движения и возвращение к ней же если движение есть
            return;

        Vector2 rounded = Utils.GetEightDirection(direction); //получение окрушлённых направлений 
        //ниже округление и вызов обновления анимации

        movement = rounded;
        lastDirection = rounded;

        UpdateAnimation();
    }
}