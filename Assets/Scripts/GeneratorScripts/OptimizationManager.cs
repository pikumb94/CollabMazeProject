using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using Priority_Queue;
using System.IO;
using System.Globalization;

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
    private double returnEval;
    private int returnIter;
    private SimplePriorityQueue<Dictionary<int, StructuredAlias>> AliasChallengePriorityQueue;
    private int maxIterations = 300;

    private List<Tuple<float,float>> graphPlot = new List<Tuple<float, float>>();
    private int iterationRandomRestart;
    private int iterationHCSwap;

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

        iterationHCSwap = 0;
        //

        while (true)
        {
            Dictionary<int, StructuredAlias> toSwapAliases = AliasGeneratorManager.Instance.GenerateNRandomAliasFromRealMap(realMap, BatchAliases);

            double maxEval = double.MinValue;
            Dictionary<int, StructuredAlias> nextNode=null;
            double currEval = Eval(realMap, currentNode);

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
                        
                        returnIter = iterations;
                        returnEval = currEval;
                        return currentNode;
                    }

                    if (tmpEval > maxEval)
                    {
                        maxEval = tmpEval;
                        nextNode = tmpAliases;
                    }

                    //
                    if (maxEval <= currEval)
                    {
                        graphPlot.Add(new Tuple<float, float>(iterationRandomRestart,Mathf.Abs((float)currEval)));
                    }
                    else
                    {
                        graphPlot.Add(new Tuple<float, float>(iterationRandomRestart, Mathf.Abs((float)maxEval)));
                    }
                    //
                    iterationHCSwap++;
                }

            }

            if (maxEval <= currEval)
            {

                returnIter = iterations;
                returnEval = currEval;
                return currentNode;
            }
                

            currentNode = nextNode;
            iterations++;
        }


    }

    public Dictionary<int, StructuredAlias> StochasticFirstChoiceHillClimber(StructuredAlias realMap, EvaluateNode Eval)
    {

        Dictionary<int, StructuredAlias> currentNode = new Dictionary<int, StructuredAlias>();
        //inizialization: take BATCH_ALIASES random alias
        currentNode = AliasGeneratorManager.Instance.GenerateNRandomAliasFromRealMap(realMap, ParameterManager.Instance.aliasNum);

        //
        int iterations = 0;
        TimeCap = ParameterManager.Instance.timeCap;

        StopwatchProxy.Instance.Stopwatch.Start();
        //

        while (iterations< maxIterations)
        {
            Tuple<int, StructuredAlias> toSwapAlias = AliasGeneratorManager.Instance.Generate1RandomAliasFromRealMap(realMap);

            double maxEval = double.MinValue;
            Dictionary<int, StructuredAlias> nextNode = null;
            double currEval = Eval(realMap, currentNode);

            foreach (var m in currentNode)
            {

                Dictionary<int, StructuredAlias> tmpAliases = new Dictionary<int, StructuredAlias>(currentNode);
                tmpAliases.Remove(m.Key);
                tmpAliases.Add(toSwapAlias.Item1, toSwapAlias.Item2);
                double tmpEval;

                try
                {
                    tmpEval = Eval(realMap, tmpAliases);
                }

                catch (Exception e)
                {
                    ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, e.Message + StopwatchProxy.Instance.Stopwatch.ElapsedMilliseconds / 1000f + "s #iteration: " + iterations+SaveAliasChallengeOptimization());
                    StopwatchProxy.Instance.Stopwatch.Stop();
                    StopwatchProxy.Instance.Stopwatch.Reset();

                    returnEval = currEval;
                    return currentNode;
                }

                if (tmpEval > maxEval)
                {
                    maxEval = tmpEval;
                    nextNode = tmpAliases;
                }

                

            }

            if (maxEval <= currEval)
            {

                returnEval = currEval;
            }
            else
            {
                returnEval = maxEval;
                currentNode = nextNode;
            }
            
            iterations++;
        }


        GeneratorUIManager.Instance.showMessageDialogBox("Execution time: " + StopwatchProxy.Instance.Stopwatch.ElapsedMilliseconds / 1000f + "s #iteration: " + iterations+ SaveAliasChallengeOptimization());
        StopwatchProxy.Instance.Stopwatch.Stop();
        StopwatchProxy.Instance.Stopwatch.Reset();

        return currentNode;
    }

    //HillClimber improve the evaluation so MAXIMIZE!
    public Dictionary<int, StructuredAlias> RandomRestartHillClimber(StructuredAlias realMap, EvaluateNode Eval)
    {

        AliasChallengePriorityQueue = new SimplePriorityQueue<Dictionary<int, StructuredAlias>>();
        iterationRandomRestart = 0;
        graphPlot.Clear();
        ParameterManager pMan = ParameterManager.Instance;
        
        System.Diagnostics.Stopwatch sWatch = StopwatchProxy.Instance.Stopwatch;
        sWatch.Stop();
        sWatch.Reset();
        sWatch.Start();

        int totalIterations = 0;
        while (iterationRandomRestart < maxIterations)
        {
            try
            {
                if (sWatch.ElapsedMilliseconds > pMan.timeCap * 1000f && pMan.timeCap>=0 )
                {
                    throw new Exception("Time cap elapsed.\n");
                }
                AliasChallengePriorityQueue.Enqueue(HillClimber(realMap, Eval), -(float) returnEval);
                //graphPlot.Add(-AliasChallengePriorityQueue.GetPriority(AliasChallengePriorityQueue.First));
            }
            catch (Exception e)
            {

                ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, e.Message + sWatch.ElapsedMilliseconds / 1000f + "s #iteration: " + iterationRandomRestart + " (" + totalIterations + returnIter +")."+ SaveAliasChallengeOptimization());
                sWatch.Stop();
                sWatch.Reset();
                return AliasChallengePriorityQueue.Dequeue();
            }
            totalIterations +=returnIter;
            iterationRandomRestart++;
        }

        GeneratorUIManager.Instance.showMessageDialogBox("F= "+ Mathf.Abs(AliasChallengePriorityQueue.GetPriority(AliasChallengePriorityQueue.First)) + "\nExecution time: " + sWatch.ElapsedMilliseconds / 1000f + "s #iteration: " + iterationRandomRestart + " ("+ maxIterations * pMan.hillClimberNumBatch +")."+ SaveAliasChallengeOptimization());
        sWatch.Stop();
        sWatch.Reset();
        return AliasChallengePriorityQueue.Dequeue();
    }

    public string SaveAliasChallengeOptimization()
    {
        string textFilePath = Application.persistentDataPath;
        ParameterManager pMan = ParameterManager.Instance;
        string returnString = "";

        if (textFilePath == null && !Directory.Exists(textFilePath))
        {
            ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, "Error while retrieving the folder, please insert a " + "valid path.");
        }
        else
        {
            try
            {
                string textMap = "";
                textMap+= "RandomRestartIteration,"+objectiveFunctionName()+"\n";
                foreach (var v in graphPlot)
                    textMap += Mathf.Abs(v.Item1) +","+v.Item2.ToString(new CultureInfo("en-US")) +"\n";

                //string fileName = pMan.aliasNum+"_"+pMan.minStepsSolution +"_" +(pMan.isBestPathOnlyExplorative? 0: pMan.onlyBestPath? 1:2)+"-" + pMan.optimizerType+"_"+pMan.hillClimberNumBatch+ "OptValues.txt";
                string fileName = "Opt"+ pMan.optimizerType+"BestP"+ (pMan.isBestPathOnlyExplorative ? 0 : pMan.onlyBestPath ? 1 : 2)+"-"+ pMan.MapToPlay.GetLength(0)+"x"+ pMan.MapToPlay.GetLength(1)+"_"+ pMan.aliasNum+"_"+ pMan.hillClimberNumBatch +"@" +$@"{DateTime.Now.Ticks}.csv";
                File.WriteAllText(@textFilePath + "/" + fileName, textMap);
                returnString = "\n" + "Optimization values \"" + fileName + "\" successfully saved at:\n" + textFilePath;

            }
            catch (Exception)
            {
                ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, "Error while saving the map at " + @textFilePath + ", please insert a valid path and check its permissions. ");
            }
        }

        return returnString;
    }

    private string objectiveFunctionName()
    {

        switch (ParameterManager.Instance.optimizerType)
        {
        
            case 0:
                return "EvalFirstZero(Min)";
            case 1:
                return "EvalForkBestPath(Max)";
            case 2:
                return "EvalForkAgents(Max)";
            case 3:
                return "EvalForkOverall(Max)";
            case 4:
                return "EvalReliabilityBestPath(Min)";
            case 5:
                return "EvalReliabilityBestPath(Max)";
            default:
                ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, "Optimization type not found.");
                return "";
        }
    }

    int randomIterations=2100;
    public Dictionary<int, StructuredAlias> PurelyRandom(StructuredAlias realMap, EvaluateNode Eval)
    {

        AliasChallengePriorityQueue = new SimplePriorityQueue<Dictionary<int, StructuredAlias>>();
        iterationRandomRestart = 0;
        graphPlot.Clear();

        ParameterManager pMan = ParameterManager.Instance;
        TimeCap = pMan.timeCap;
        BatchAliases = pMan.aliasNum;
        System.Diagnostics.Stopwatch sWatch = StopwatchProxy.Instance.Stopwatch;
        AliasGeneratorManager aGMan = AliasGeneratorManager.Instance;
        sWatch.Stop();
        sWatch.Reset();
        sWatch.Start();

        int totalIterations = 0;
        while (totalIterations<randomIterations)
        {
            Dictionary<int, StructuredAlias> randomAliases = aGMan.GenerateNRandomAliasFromRealMap(realMap, BatchAliases);

            try
            {
                if (sWatch.ElapsedMilliseconds > pMan.timeCap * 1000f && pMan.timeCap >= 0)
                {
                    throw new Exception("Time cap elapsed.\n");
                }
                float tmpEval = (float) Eval(realMap, randomAliases);
                AliasChallengePriorityQueue.Enqueue(randomAliases, -(float)tmpEval);
                graphPlot.Add(new Tuple<float, float>(iterationRandomRestart, Mathf.Abs((float)tmpEval)));
            }
            catch (Exception e)
            {

                ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, e.Message + sWatch.ElapsedMilliseconds / 1000f + "s #iteration: " + totalIterations  + SaveAliasPurelyRandom());
                sWatch.Stop();
                sWatch.Reset();
                return AliasChallengePriorityQueue.Dequeue();
            }
            totalIterations++;
        }
        


        GeneratorUIManager.Instance.showMessageDialogBox("SIMPLERANDOM-F= " + Mathf.Abs(AliasChallengePriorityQueue.GetPriority(AliasChallengePriorityQueue.First)) + "\nExecution time: " + sWatch.ElapsedMilliseconds / 1000f + "s #iteration: " + totalIterations + SaveAliasPurelyRandom());
        sWatch.Stop();
        sWatch.Reset();
        return AliasChallengePriorityQueue.Dequeue();
    }

    public string SaveAliasPurelyRandom()
    {
        string textFilePath = Application.persistentDataPath;
        ParameterManager pMan = ParameterManager.Instance;
        string returnString = "";

        if (textFilePath == null && !Directory.Exists(textFilePath))
        {
            ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, "Error while retrieving the folder, please insert a " + "valid path.");
        }
        else
        {
            try
            {
                string textMap = "";
                textMap +=  objectiveFunctionName() + "\n";
                foreach (var v in graphPlot)
                    textMap += v.Item2.ToString(new CultureInfo("en-US")) + "\n";

                string fileName = "Opt" + pMan.optimizerType + "BestP" + (pMan.isBestPathOnlyExplorative ? 0 : pMan.onlyBestPath ? 1 : 2) + "-" + pMan.MapToPlay.GetLength(0) + "x" + pMan.MapToPlay.GetLength(1) + "_" + pMan.aliasNum + "_" + pMan.hillClimberNumBatch + "@" + graphPlot.Count + "PURELYRANDOM"+$@"{DateTime.Now.Ticks}"+".csv";
                File.WriteAllText(@textFilePath + "/" + fileName, textMap);
                returnString = "\n" + "Optimization values \"" + fileName + "\" successfully saved at:\n" + textFilePath;

            }
            catch (Exception)
            {
                ErrorManager.ManageError(ErrorManager.Error.SOFT_ERROR, "Error while saving the map at " + @textFilePath + ", please insert a valid path and check its permissions. ");
            }
        }

        return returnString;
    }
}
