using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace TestingServerApp
{
    public class ExternalAnswersTypetemplateSelector : DataTemplateSelector
    {
        public DataTemplate SingleQuestionDateTemplate { get; set; }
        public DataTemplate MultipleQuestionsDateTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is AnswerVM answer && answer.MultipleAnswersAllowed == true)
            {
                return MultipleQuestionsDateTemplate;
            }
            else
            {
                return SingleQuestionDateTemplate;
            }
        }
    }
}
