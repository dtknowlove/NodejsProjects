using UnityEngine;
using System.Collections;

namespace PTGame.Utils.CVS
{
	public interface IDataConfigBase {

		void InitData(string csvFile);
		string GetCsvFile();
	}
}
