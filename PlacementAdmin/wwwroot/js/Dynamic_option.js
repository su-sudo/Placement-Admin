let optionIndex = 1;
function addOption() {
    const optionsContainer = document.getElementById('options-container');
    const newOption = document.createElement('div');
    newOption.classList.add('option-item');
    newOption.innerHTML = `
        <input type="text" name="Options[${optionIndex}].OptionText" placeholder="Option ${optionIndex + 1}" class="form-control mb-2" />
        <input type="hidden" name="Options[${optionIndex}].IsCorrect" value="false" />
        <input type="checkbox" name="Options[${optionIndex}].IsCorrect" value="true" /> Correct
    `;
    optionsContainer.appendChild(newOption);
    optionIndex++;
}

function toggleQuestionType() {
    const questionType = document.querySelector('select[name="QuestionType"]').value;
    const optionsContainer = document.getElementById('options-container');
    const codingAnswerContainer = document.getElementById('CodingAnswerField');

    if (questionType === 'MCQ') {
        optionsContainer.style.display = 'block';
        codingAnswerContainer.style.display = 'none';
    }
    else if (questionType === 'Code') {
        optionsContainer.style.display = 'none';
        codingAnswerContainer.style.display = 'block';
    }
}

document.addEventListener('DOMContentLoaded', function () {
    toggleQuestionType();
});

document.addEventListener('DOMContentLoaded', function (event)
{
    var successAlert = document.getElementById('success-alert');
    var observer = new MutationObserver(function (mutations) {
        mutations.forEach(function (mutation) {
            if (mutation.addedNodes.length && Array.from(mutation.addedNodes).some(node => node.id === "success-alert")) {
                successAlert.style.display = 'block';
                setTimeout(function () {
                    successAlert.style.display = 'none';
                },5000);
            }
        });
    });
    observer.observe(document.documentElement, {
        childList: true,
        subtree: true
    });
});
