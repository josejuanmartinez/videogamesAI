import os

if __name__ == "__main__":
    rename = dict()
    folder_path = 'Assets/Art/Cards/Decks'
    with open('images_used.txt', 'r') as f:
        for line in f.readlines():
            fn = "-".join(line.split('-')[:-1])
            rename[fn] = "-".join(line.split('-')[-1:]).strip()

    # List all files in the folder
    files = os.listdir(folder_path)

    # Iterate through each file
    for file_name in files:
        file_name_no_ext = os.path.splitext(os.path.basename(file_name))[0]
        ext = file_name.replace(file_name_no_ext, '')
        file_path = os.path.join(folder_path, file_name)

        # Check if the file is not in the 'used' list
        if file_name_no_ext in rename:
            # Remove the file
            try:
                os.rename(f"{folder_path}/{file_name}", f"{folder_path}/{rename[file_name_no_ext]}{ext}")
            except:
                pass
            try:
                os.rename(f"{folder_path}/{file_name.replace(ext, ext+'.meta')}", f"{folder_path}/{rename[file_name_no_ext]}{ext+'.meta'}")
            except:
                pass

