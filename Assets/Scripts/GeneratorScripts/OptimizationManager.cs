using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptimizationManager : MonoBehaviour
{
    static readonly int BATCH_ALIASES = 10;//should not be greater than MAX_OPT_ALIAS
    public delegate double EvaluateNode(StructuredAlias realMap, Dictionary<int, StructuredAlias> alias);
    
    //HillClimber improve the evaluation so MAXIMIZE!
    public Dictionary<int, StructuredAlias> HillClimber(StructuredAlias realMap, EvaluateNode Eval)
    {

        Dictionary<int, StructuredAlias> currentNode = new Dictionary<int, StructuredAlias>();
        //inizialization: take BATCH_ALIASES random alias
        currentNode = AliasGeneratorManager.Instance.GenerateNRandomAliasFromRealMap(realMap,ParameterManager.Instance.aliasNum);

        while (true)
        {
            Dictionary<int, StructuredAlias> toSwapAliases = AliasGeneratorManager.Instance.GenerateNRandomAliasFromRealMap(realMap, BATCH_ALIASES);

            double maxEval = double.MinValue;
            Dictionary<int, StructuredAlias> nextNode=null;

            foreach (var m in currentNode)
            {
                foreach(var n in toSwapAliases)
                {
                    Dictionary<int, StructuredAlias>  tmpAliases = new Dictionary<int, StructuredAlias>(currentNode);
                    tmpAliases.Remove(m.Key);
                    tmpAliases.Add(n.Key, n.Value);

                    double tmpEval = Eval(realMap, tmpAliases);

                    if (tmpEval > maxEval)
                    {
                        maxEval = tmpEval;
                        nextNode = tmpAliases;
                    }
                        
                }
                    
            }

            if (maxEval <= Eval(realMap, currentNode))
                return currentNode;

            currentNode = nextNode;

        }


    }
}
