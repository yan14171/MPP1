var curLine = "";
var termFreq = new KeyValuePair<string, int>[1000000];
var lastWordEndIndex = 0;
var symbolIndex = 0;
var curWordIndex = 0;
var wordsInFreq = 0;

var realWordsCount = 0;

var inputStream = new StreamReader("in.txt");
var outputStream = new StreamWriter("out.txt");

start:
curLine = inputStream.ReadLine();
if (curLine == "")
	goto end;
lastWordEndIndex = 0;
curWordIndex = 0;
var words = new string[curLine.Length];
splitStart:

if (symbolIndex == curLine.Length)
{
	words[curWordIndex++] = curLine[lastWordEndIndex..symbolIndex];
	symbolIndex = 0;
	realWordsCount++;
	goto splitEnd;
}
if (curLine[symbolIndex] == ' ')
{
	words[curWordIndex++] = curLine[lastWordEndIndex..symbolIndex];
	lastWordEndIndex = ++symbolIndex;
	realWordsCount++;
	goto splitStart;
}
if ((int)curLine[symbolIndex] >= 0x41 && (int)curLine[symbolIndex] <= 0x5A)
{
	var oldChar_UTF16LE = (int)curLine[symbolIndex];
	var newChar_UTF16LE = oldChar_UTF16LE + 0x20;
	var newChar = $"{(char)newChar_UTF16LE}";

	curLine = curLine[0..(symbolIndex)] + newChar + curLine[(symbolIndex + 1)..curLine.Length];

	symbolIndex++;
	goto splitStart;
}
symbolIndex++;
goto splitStart;

splitEnd:
var wordCounter = 0;
var incrementWordsCount = 0;

operateWord:
if (words[wordCounter] is null)
{
	wordsInFreq = wordsInFreq + realWordsCount;
	goto start;
}
bool termFreqContainsKey = false;
var checkIndex = 0;
containsCheck:
if (termFreq[checkIndex].Key == words[wordCounter])
{
	termFreqContainsKey = true;
}
else if (checkIndex < (realWordsCount - 1))
{
	checkIndex++;
	goto containsCheck;
}

if (termFreqContainsKey)
{
	incrementWordsCount++;
	termFreq[checkIndex] = new KeyValuePair<string, int>(termFreq[checkIndex].Key, termFreq[checkIndex].Value + 1);
}
else
	termFreq[wordsInFreq + wordCounter - incrementWordsCount] = new KeyValuePair<string, int>(words[wordCounter], 1);

wordCounter++;
goto operateWord;
end:


int i, j, arr_size = realWordsCount;
WordInfo temp;
var arr = termFreq;
i = 0;
startouter:
if (i >= arr_size)
	goto endouter;
j = 0;
startinner:
if (j >= arr_size - 1)
	goto endinner;

if (arr[j].Value >= arr[j + 1].Value)
	goto noswap;
temp = new WordInfo(arr[j].Key, arr[j].Value);
arr[j] = arr[j + 1];
arr[i + 1] = new KeyValuePair<string, int>(temp.Word, temp.Frequency);
noswap:
j = j + 1;
goto startinner;
endinner:
i = i + 1;
goto startouter;
endouter:

var outputCounter = 0;
endLoop:
if (arr[outputCounter].Key is null)
	goto endEnd;
outputStream.WriteLine(arr[outputCounter].Key + " - " + arr[outputCounter].Value);
outputCounter++;
if (outputCounter < realWordsCount)
	goto endLoop;
endEnd:
outputStream.WriteLine("------");
inputStream.Close();
outputStream.Close();

struct WordInfo
{
	public string Word;
	public int Frequency;

	public WordInfo(string v1, int v2) : this()
	{
		this.Word = v1;
		this.Frequency = v2;
	}
}




