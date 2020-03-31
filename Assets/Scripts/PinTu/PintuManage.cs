using System;
using System.Collections.Generic;
using UnityEngine;

public class PinTuManager
{
    private static PinTuManager instance;
    public static PinTuManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new PinTuManager();
            }
            return instance;
        }
    }

     //summary
     //随机数组
     //summary
     //typeparam name=Ttypeparam
     //param name=arrayparam
     //returnsreturns
    public int[] ArrayRandom(int[] array)
    {
        System.Random random = new System.Random();
        int[] newArray = new int[array.Length];
        for (var i = 0; i<array.Length; i++)
        {
            var _random = random.Next(i,array.Length);
            var tmp = array[i];
            array[i] = array[_random];
            array[_random] = tmp;
        }

        return array;
    }

     //summary
     //返回数组的逆序数量
     //summary
     //param name=arrayparam
     //returnsreturns
    public int Result(int[] array)
    {
        int count = 0;
        int index = -1;
        for (int i = 0; i<array.Length; i++)
        {
            if(array[i] == array.Length-1)
            {
                index = array.Length - 1 - i;
            }
            for (int j = i+1; j<array.Length; j++)
            {
                if (array[i]>array[j])
                {
                    count++;
                }
            }

            
        }
        if(index == -1)
        {
            return -1;
        }
        int a = count + index;
        //Debug.Log("AAAAAAAAAAAAAAA" + a);
        return (count + index);
    }








}
