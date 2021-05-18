public class list
{
    // robots linked list
    //  prev <--   fixed order    --> next
    //  prev <--sorted by x coord --> next
    public node head = new node();

    public node last = new node();
    private node pcurr = null;

    public list()
    {
        head.robn = 0;
        last.robn = 0;
        head.xpos = -10000;
        last.xpos = 99999999;
        head.pn = last;
        head.orn = last;
        head.pp = head;
        head.orp = head;
        last.pn = last;
        last.orn = last;
        last.pp = head;
        last.orp = head;
        pcurr = head;
    }

    public void addrobot(int ind, int xpos)
    {
        node k = null;

        var curr = new node();
        last.pp.pn = curr;
        curr.pp = last.pp;
        curr.pn = last;
        last.pp = curr;
        curr.robn = ind;
        curr.xpos = xpos;
        k = head;
        while (k.xpos < xpos && k.xpos > 0)
        {
            k = k.orn;
        }
        curr.orn = k;
        curr.orp = k.orp;
        k.orp.orn = curr;
        k.orp = curr;
    }

    public void deleteall()
    {
        head.robn = 0;
        last.robn = 0;
        head.xpos = -10000;
        last.xpos = 99999999;
        head.pn = last;
        head.orn = last;
        head.pp = head;
        head.orp = head;
        last.pn = last;
        last.orn = last;
        last.pp = head;
        last.orp = head;
        pcurr = head;
    }

    public void delrobot(node rob)
    {
        rob.pp.pn = rob.pn;
        rob.orp.orn = rob.orn;
        rob.pn.pp = rob.pp;
        rob.orn.orp = rob.orp;
    }

    public node firstnode()
    {
        node firstnode = null;
        firstnode = head.pn;
        return firstnode;
    }

    public node firstorder()
    {
        node firstorder = null;
        firstorder = head.orn;
        return firstorder;
    }

    public node firstpos(int rsize, decimal xpos)
    {
        node firstpos = null;
        node n = null;

        n = head.orn;
        while (n.xpos + rsize < xpos && (n != last))
        {
            n = n.orn;
        }
        firstpos = n;
        return firstpos;
    }

    public node firstprox(node n, int dist)
    {
        node firstprox = null;
        int xcurr = 0;

        node ncurr = null;

        ncurr = n;
        xcurr = n.xpos;
        n = n.orp;
        while (n.xpos + dist >= xcurr)
        {
            n = n.orp;
        }
        firstprox = n.orn;
        return firstprox;
    }

    public node firstrob()
    {
        node firstrob = null;
        firstrob = head.pn;
        return firstrob;
    }

    public node nextnode(node n)
    {
        node nextnode = null;
        nextnode = n.pn;
        return nextnode;
    }

    public node nextorder(node n)
    {
        node nextorder = null;
        nextorder = n.orn;
        return nextorder;
    }

    public node prevnode(node n)
    {
        node prevnode = null;
        prevnode = n.pp;
        return prevnode;
    }

    public node prevorder(node n)
    {
        node prevorder = null;
        prevorder = n.orp;
        return prevorder;
    }

    public void printmain()
    {
        node tmp = null;

        tmp = head.pn;
        while (tmp != last)
        {
            //Debug.Print tmp.robn, tmp.xpos
            tmp = tmp.pn;
        }
    }

    public void printorder()
    {
        node tmp = null;

        tmp = head.orn;
        while (tmp != last)
        {
            //Debug.Print tmp.robn, tmp.xpos
            tmp = tmp.orn;
        }
    }

    public node searchrob(int ind)
    {
        node searchrob = null;
        node tmp = null;

        tmp = head.pn;
        while (tmp.robn != ind && (tmp != last))
        {
            tmp = tmp.pn;
        }
        searchrob = tmp;
        return searchrob;
    }

    public void stay(node rob, int x)
    {
        node tmp = null;

        if (x < 0)
        {
            x = 0;
        }
        rob.xpos = x;
        int counter = 0;

        if (rob.xpos > rob.orn.xpos)
        {
            tmp = rob.orn;
            while (rob.xpos > tmp.xpos)
            {
                tmp = tmp.orn;
                counter = counter + 1;
                if (counter > 10000)
                {
                    return;
                }
            }
            rob.orp.orn = rob.orn;
            rob.orn.orp = rob.orp;
            tmp.orp.orn = rob;
            rob.orp = tmp.orp;
            tmp.orp = rob;
            rob.orn = tmp;
        }
        else
        {
            if (rob.xpos < rob.orp.xpos)
            {
                tmp = rob.orp;
                while (rob.xpos < tmp.xpos)
                {
                    tmp = tmp.orp;
                    counter = counter + 1;
                    if (counter > 10000)
                    {
                        return;
                    }
                }
                rob.orp.orn = rob.orn;
                rob.orn.orp = rob.orp;
                rob.orp = tmp;
                rob.orn = tmp.orn;
                tmp.orn = rob;
                rob.orn.orp = rob;
            }
        }
    }
}
