'''
Generated by Leslie @Leadrive in Shanghai

Time：2019年6月26日16:40:40
Kep on mvi

'''

import os
import csv
import logging

def Search_Str(lines, search_str, condi_code=''):
	"""找寻首部为search_str的行,以行字符串返回，返回值为列表lines_search
		同时可以附加寻找的条件:该行是否有特定的子字符串
		BA , BO DBC文件
	"""
	
	lines_search = []	
	
	for line in lines:
		string = line.split()
		if string and string[0].strip() == search_str:
			if not condi_code:
				lines_search.append(line)
			elif condi_code in string:
				lines_search.append(line)
			else:
				pass		
	
	return lines_search

def OutIDandAttriDic(lines_search, pos_id, pos_attribute):
	"""在列表中寻找帧ID及其某个属性，并以字典形式返回 """
	
	idandattridic = {}
	
	for line in lines_search:
		string = line.split()
		if len(string)>max([pos_id, pos_attribute]):
			if ':' in string[pos_attribute]:
				idandattridic[string[pos_id]] = string[pos_attribute].split(':')[0]
			elif ';' in string[pos_attribute]:
				idandattridic[string[pos_id]] = string[pos_attribute].split(';')[0]
			else:
				idandattridic[string[pos_id]] = string[pos_attribute]
			
	return idandattridic

def OutIdandName(dbclines):
	"""output the message information in the format of dic"""
	
	idandNameDic = {}
	header_string = 'BO_'
	BO_lines = Search_Str(dbclines, header_string)
	idandNameDic = OutIDandAttriDic(BO_lines, 1, 2)
	
	return idandNameDic
	
def OutIdandTime(dbclines):
	"""output the message information in the format of dic"""
	
	idandTimeDic = {}
	header_string = 'BA_'
	str_condition = '"GenMsgCycleTime"'
		
	BA_lines = Search_Str(dbclines, header_string, condi_code=str_condition)
	idandTimeDic = OutIDandAttriDic(BA_lines, 3, 4)
	
	return idandTimeDic
	
def Create_csv(listInfo, mode):
	path = "C:\\Users\\shawphon\\Desktop\\Config.csv"    
	with open(path, mode, newline='') as f:
		csv_write = csv.writer(f)
		csv_head = [listInfo[0], listInfo[1], listInfo[2], listInfo[3]]
		csv_write.writerow(csv_head)

def Read_File(file_name):
	"""读取文件内容，返回行列表lines"""
	
	with open(file_name) as file_object:
		lines = file_object.readlines()
#		print(file_name + ": 文件读取成功！\t\t\t\t\t\t#")
		
	return lines
	

def CreatCSVFile(fileName):
	"""根据DBC文件生成相应的CSV文件"""
		
	
	listColumn = ["MessageID", "SignalName", "Period", "GroupName"]
	listInfo = []
	Create_csv(listColumn, 'w')		#添加列表列名
	lines = Read_File(fileName)
	
	dicTime = {}
	dicTime = OutIdandTime(lines)#消息名及其周期组成字典
	
	lineSearched = Search_Str(lines, "BO_")
	for i in range(len(lineSearched)-1):
		firstBO = lines.index(lineSearched[i])
		secondBO = lines.index(lineSearched[i+1])
		
		if lines[firstBO].strip().split()[-1] == "ISGCM":			
			messageID = lines[firstBO].strip().split()[1]
			period = dicTime[messageID]
			for j in range(firstBO, secondBO):
				if "SG_" in lines[j]:
					listInfo.append(messageID)#添加messageID
					listInfo.append(lines[j].strip().split()[1])#添加SignalName
					listInfo.append(period)#添加Period
					listInfo.append("Received")
					Create_csv(listInfo, 'a')
					listInfo.clear()
				
		if lines[firstBO].strip().split()[-1] == "HCU":
			messageID = lines[firstBO].strip().split()[1]
			period = dicTime[messageID]
			for j in range(firstBO, secondBO):
				flag = 0
				for k in range(firstBO, secondBO):
					if "ISGCM" in lines[k]:
						flag = 1
						break				
				if "SG_" in lines[j] and flag == 1:
					listInfo.append(messageID)#添加messageID
					listInfo.append(lines[j].strip().split()[1])#添加SignalName
					listInfo.append(period)#添加Period
					listInfo.append("Transmitted")
					Create_csv(listInfo, 'a')
					listInfo.clear()
					
	firstBO = lines.index(lineSearched[i+1])
	secondBO = len(lines)-1
	if lines[firstBO].strip().split()[-1] == "ISGCM":		
		messageID = lines[firstBO].strip().split()[1]
		period = dicTime[messageID]
		for j in range(firstBO, secondBO):
			if "SG" in lines[j].strip().split('_')[0]:
				listInfo.append(messageID)#添加messageID
				listInfo.append(lines[j].strip().split()[1])#添加SignalName
				listInfo.append(period)#添加Period
				listInfo.append("Received")
				Create_csv(listInfo, 'a')
				listInfo.clear()
				
	if lines[firstBO].strip().split()[-1] == "HCU":
		messageID = lines[firstBO].strip().split()[1]
		period = dicTime[messageID]
		for j in range(firstBO, secondBO):
			flag = 0
			for k in range(firstBO, secondBO):
				if "ISGCM" in lines[k]:
					flag = 1
					break				
			if "SG" in lines[j] and flag == 1:
				listInfo.append(messageID)#添加messageID
				listInfo.append(lines[j].strip().split()[1])#添加SignalName
				listInfo.append(period)#添加Period
				listInfo.append("Transmitted")
				Create_csv(listInfo, 'a')
				listInfo.clear()
if __name__=='__main__':
	"""主函数部分"""
	print(os.path.abspath("."))
	CreatCSVFile(os.path.abspath(".") + "\\YC_NewEnergy_DMCM&FISGM_Matrix_V1.0_20190613.dbc")
		










