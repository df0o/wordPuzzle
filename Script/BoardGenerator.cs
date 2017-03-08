	using UnityEngine;

public class BoardGenerator {
//  private static double[] freq = {
//		0.08167,//a
//		0.01492,
//		0.02782,
//		0.04253,
//		0.12702,//e
//		0.02228,
//		0.02015,
//		0.06094,
//		0.06966,//i
//		0.00153,
//		0.00772,
//		0.04025,
//		0.02406,
//		0.06749,
//		0.07507,//o
//		0.01929,
//		0.00095, 
//		0.05987,
//		0.06327,
//		0.09056,
//		0.02758,//u
//		0.00978,
//		0.02360,
//		0.00150,
//		0.01974,
//		0.00074 + 0.00001
//	};

  private static double[] freq = {
    0.0908, //a
    0.0208, //b
    0.0208,
    0.0408,
    0.1208, //e
    0.0208,
    0.0308,
    0.0208,
    0.0908, //i
    0.0108,
    0.0108,
    0.0408, //l
    0.0208, 
    0.0608, //n
    0.0808, //o
    0.0208, 
    0.01,
    0.0608, //r
    0.0408, //s
    0.0608, //t
    0.0408, //u
    0.0208, 
    0.0208,
    0.0108,
    0.0208,
    0.0108
  };

  //abcdefghijklmnopqrstuvwxyz
  //Scrabble_letter_distributions
  private static readonly int[] score = {
		1,//a
		3,//b
		3,//c
		2,//d
		1,//e
		4,//f
		2,//g
		4,//h
		1,//i
		8,//j
		5,//k
		1,//l
		3,//m
		1,//n
		1,//o
		3,//p
		10,//q
		1,//r
		1,//s
		1,//t
		1,//u
		4,//v
		4,
		8,
		4,
		10
	};

  public static double[] freqModifier(double m) {
    double total = 0;
    double[] output = new double[26];
    for (int i = 0; i < 26; i++) {
      output[i] += freq[i] / m;
      total += output[i];
    }
    double r = (1 - total) / 5;
    output['a' - 'a'] += r;
    output['e' - 'a'] += r;
    output['i' - 'a'] += r;
    output['o' - 'a'] += r;
    output['u' - 'a'] += r;
    return output;
  }

  public static string generateRandomCharacter(int modifier=1) {
    double rand = (double)UnityEngine.Random.Range(0.0f, 10000.0f) / 10000.0;
    double total = 0.0;
    double[] freq = freqModifier(modifier);
    for (int i = 0; i < freq.Length; i++) {
      total += freq[i];
      if (rand < total) {
        return "" + (char)('a' + i);
      }
    }
    return "" + (char)('a' + freq[freq.Length - 1]);
  }
  public static int getScoreFromCharacter(string c) {
    int i = c[0] - 'a';
    return score[i];
  }
}
