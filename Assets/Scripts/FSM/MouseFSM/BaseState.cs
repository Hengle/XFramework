using System.Collections;
using System.Collections.Generic;


public class BaseState<T1,T2,T3> {
    virtual public void HandleInput(T1 t1,T2 t2,T3 t3) { }
    virtual protected void Update(T1 t1) { }
}