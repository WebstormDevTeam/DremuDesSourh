#!/bin/zsh

# 定义要忽略的文件和文件夹列表
IGNORE_LIST=(
    "[Ll]ibrary/"
    "[Oo]bj/"
    "[Tt]emp/"
    "[Bb]uild/"
    "/*.sln"
    "/*.csproj"
    "/*.meta"
    "/*.apk"
    "/*.unitypackage"
    "/sysinfo.txt"
    "/TextMesh Pro/"
    "/.gradle/"
    "/.vs/"
    # 添加其他需要忽略的文件和文件夹
)

# 创建或更新 .gitignore 文件
echo "# Unity Project .gitignore" > .gitignore
for item in "${IGNORE_LIST[@]}"; do
    echo $item >> .gitignore
done

# 从Git索引中移除这些文件
git ls-files --ignored --exclude-standard | while read file; do
    git rm --cached "$file"
done

# 提交更改
git commit -am "Remove ignored files from Git"

# 推送到远程仓库
git push origin HEAD

echo "Files have been removed from Git and .gitignore has been updated."