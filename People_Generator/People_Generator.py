"""
Program to generate SQL-Scripts for the countries Application
"""

import random

def Generate_People(countries_len, people_per_country, residence_probability):

    def generate_name(element_list, len_list):
        selected = element_list[random.randint(0, len_list - 1)]
        return selected[ : len(selected) - 2]

    def generate_birthdate():
        year = str(random.randint(1950, 2018)) + "-"
        month = str(random.randint(1, 12)) + "-"
        day = str(random.randint(1, 28))
        return year + month + day

    names_file = open("Files\\Names.txt", "r")
    last_names_file = open("Files\\LastNames.txt", "r")

    names_list = names_file.readlines()
    last_names_list = last_names_file.readlines()

    names_len = len(names_list)
    last_names_len = len(last_names_list)

    sql_file = open("Files\\sql.txt", "w")

    for i in range(1, countries_len + 1):
        for j in range(1, people_per_country + 1):
            # Name and last name
            name = generate_name(names_list, names_len)
            last_name = generate_name(last_names_list, last_names_len)

            name_instruction = "'" + name + "', "
            last_name_instruction = "'" + last_name + "', "

            # id number
            id_number_instruction = str(j) + ", "
            
            # Birth and Residence Country
            country_instruction = "'" + str(i) + "', "
            if (random.randint(0, 100) < residence_probability):
                residence_instruction = country_instruction
            else:
                residence_instruction = "'" + str(random.randint(1, countries_len)) + "', "

            # Generate birthdate
            birthdate_instruction = "'" + generate_birthdate() + "', "

            # Email
            email_instruction = "'" + name + last_name + "@hotmail.com', "

            # SQL Instruction
            sql_instruction = "INSERT INTO dbo.Person VALUES(" + id_number_instruction + name_instruction\
            + last_name_instruction + country_instruction + residence_instruction + birthdate_instruction\
            + email_instruction + "NULL, NULL)\n"

            # Write File
            sql_file.write(sql_instruction)