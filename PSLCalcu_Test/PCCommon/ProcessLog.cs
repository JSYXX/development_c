// PGIM Tools - PGIM Tools Common
// Copyright (C) 2004-2005 ABB
// 
// Creator: Jian Guan(jian.guan@cn.abb.com)
// Create date: 2006.07.12
// History:

using System;
using System.IO;
using System.Collections;

namespace PgimTools{
	/// <summary>
	/// 记录backup,restore过程中的日志
	/// </summary>
	public class ProcessLog: IDisposable{
		const string FILE_NAME = "PgimToolsProcess.log";
		const string LOG_PATH = "log";
	
		StreamWriter _writer;
	
		public ProcessLog(){
			//create log directory
			//string path = gcsl.AppG.MapPath(LOG_PATH);
			//if (!Directory.Exists(path))
			//    Directory.CreateDirectory(path);
				
			//_writer = new StreamWriter(Path.Combine(path, FILE_NAME), false);
			Init(false);
		}

		/// <summary>
		/// enable appending content
		/// </summary>
		/// <param name="append"></param>
		public ProcessLog(bool append) {
			Init(append);
		}

		void Init(bool append) {
			//create log directory
			string path = gcsl.AppG.MapPath(LOG_PATH);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			_writer = new StreamWriter(Path.Combine(path, FILE_NAME), append);
		}
		
		/// <summary>
		/// read all lines
		/// </summary>
		/// <returns></returns>
		public static string GetAllMessages(){
			string message = string.Empty;
			
			string fileName = gcsl.AppG.MapPath(string.Format("{0}\\{1}", LOG_PATH, FILE_NAME));
			if (File.Exists(fileName)){
				using(StreamReader reader = new StreamReader(fileName)){
					message = reader.ReadToEnd();
				}//using
			}//if
			
			return message;
		}
		
		public void Dispose(){
			_writer.Close();
		}
		
		/// <summary>
		/// write log
		/// </summary>
		/// <param name="message"></param>
		public void Write(string message){
			_writer.WriteLine(message);
		}

		/// <summary>
		/// write ok message
		/// </summary>
		/// <param name="message"></param>
		public void WriteOK(string message) {
			Write("[OK]"+message);
		}

		/// <summary>
		/// write error message
		/// </summary>
		/// <param name="message"></param>
		public void WriteError(string message) {
			Write("[ERROR]" + message);
		}
	}
}
