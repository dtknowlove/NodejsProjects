#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

echo $DIR
ProjectName=$1 #工程名称
CopyCount=$2 #个数

if (( $CopyCount < 1 )); then
  echo "copy数量不能小于1"
  exit 0
fi

root_src="$DIR/$ProjectName";
root_src_Library="${root_src}/Library"

MakeLink(){
  id=$1;
  root_copy="$DIR/${ProjectName}_${id}"

  if [ ! -d $root_copy ]; then
    mkdir $root_copy
  fi

  # link Assets ProjectSettings obj
  link_array=("Assets" "ProjectSettings")
  for dirname in ${link_array[@]}; do
      ln -s "${root_src}/${dirname}" "${root_copy}/${dirname}"
  done

  # link libaray
  root_copy_Library="${root_copy}/Library"
  if [ ! -d $root_copy_Library ]; then
    mkdir $root_copy_Library
  fi

  files=$(ls $root_src_Library)

  for filename in $files
  do
    if [ "$filename" != "EditorInstance.json" ]; then
        ln -s "${root_src_Library}/${filename}" "${root_copy_Library}/${filename}"
    fi
    
  done
}

for((i=1;i<(${CopyCount}+1);i++)); do
  MakeLink $i
done

echo "done"


