using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
public class StopwatchProxy
{
    private Stopwatch _stopwatch;
    private static readonly StopwatchProxy _stopwatchProxy = new StopwatchProxy();

    private StopwatchProxy()
    {
        _stopwatch = new Stopwatch();
    }

    public Stopwatch Stopwatch { get { return _stopwatch; } }

    public static StopwatchProxy Instance {
        get { return _stopwatchProxy; }
    }
}

public class OptimizationManager : MonoBehaviour
{
    private int BatchAliases = 10;//should not be greater than MAX_OPT_ALIAS
    public delegate double EvaluateNode(StructuredAlias realMap, Dictionary<int, StructuredAlias> alias);
    private int TimeCap;


    //HillClimber improve the evaluation so MAXIMIZE!
    public Dictionary<int, StructuredAlias> HillClimber(StructuredAlias realMap, EvaluateNode Eval)
    {

        Dictionary<int, StructuredAlias> currentNode = new Dictionary<int, StructuredAlias>();
        //inizialization: take BATCH_ALIASES random alias
        currentNode = AliasGeneratorManager.Instance.GenerateNRandomAliasFromRealMap(realMap,ParameterManager.Instance.aliasNum);

        //
        int iterations = 0;
        TimeCap = ParameterManager.Instance.timeCap;
        BatchAliases = ParameterManager.Instance.hillClimberNumBatch;
        
        StopwatchProxy.Instance.Stopwatch.Start();
        //

        while (true)
        {
            Dictionary<int, StructuredAlias> toSwapAliases = AliasGeneratorManager.Instance.GenerateNRandomAliasFromRealMap(realMap, BatchAliases);

            double maxEval = double.MinValue;
            Dictionary<int, StructuredAlias> nextNode=null;

            foreach (var m in currentNode)
            {
                foreach(var n in toSwapAliases)
                {
                    Dictionary<int, StructuredAlias>  tmpAliases = new Dictionary<int, StructuredAlias>(currentNode);
                    tmpAliases.Remove(m.Key);
                    tmpAliases.Add(n.Key, n.Value);
                    double tmpEval;

                    try{
                        tmpEval = Eval(realMap, tmpAliases);
                    }
                    catch (Exception e){
                        ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, e.Message + StopwatchProxy.Instance.Stopwatch.ElapsedMilliseconds / 1000f + "s #iteration: " + iterations);
                        StopwatchProxy.Instance.Stopwatch.Stop();
                        StopwatchProxy.Instance.Stopwatch.Reset();
                        
                        return currentNode;
                    }

                    if (tmpEval > maxEval)
                    {
                        maxEval = tmpEval;
                        nextNode = tmpAliases;
                    }
                        
                }

            }

            if (maxEval <= Eval(realMap, currentNode))
            {
                GeneratorUIManager.Instance.showMessageDialogBox("Execution time: " + StopwatchProxy.Instance.Stopwatch.ElapsedMilliseconds / 1000f + "s #iteration: " + iterations);
                StopwatchProxy.Instance.Stopwatch.Stop();
                StopwatchProxy.Instance.Stopwatch.Reset();
                
                return currentNode;
            }
                

            currentNode = nextNode;
            iterations++;
        }


    }
}
